using System;

//using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Reflection;
using ReflexApi.SteamData;
using ReflexApi.Util;

namespace ReflexApi
{
    /// <summary>
    /// Class for retrieving country information from SQLite DB.
    /// </summary>
    public class HostCountryRetriever
    {
        private static readonly Type LogClassType = MethodBase.GetCurrentMethod().DeclaringType;

        //private readonly SQLiteConnection _conn;
        private readonly Mono.Data.Sqlite.SqliteConnection _conn;

        private readonly string _sqlConString = "Data Source=" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
           "countries.sqlite");

        /// <summary>
        /// Initializes a new instance of the <see cref="HostCountryRetriever"/> class.
        /// </summary>
        public HostCountryRetriever()
        {
            //_conn = new SQLiteConnection(_sqlConString);
            _conn = new Mono.Data.Sqlite.SqliteConnection(_sqlConString);
            _conn.Open();
        }

        /// <summary>
        /// Gets the country information.
        /// </summary>
        /// <param name="ipe">The server's ip endpoint.</param>
        /// <returns>The server's country information as a <see cref="CountryData"/> object.</returns>
        public CountryData GetCountryInfo(IPEndPoint ipe)
        {
            IPAddress ipa;
            string ipStr = ipe.Address.ToString();
            try
            {
                ipa = IPAddress.Parse(ipStr);
            }
            catch (Exception ex)
            {
                LoggerUtil.LogInfoAndDebug(string.Format("Country retrieval: unable to parse ip {0}, exception: {1}"
                    , ipStr, ex), LogClassType);
                return SetUnknownCountryData();
            }

            var bytes = ipa.GetAddressBytes();

            // IP address needs to be a signed 64-bit int, as that is how database stores it
            var convertedIp = (long)(16777216 * (long)bytes[0] + 65536 * (long)bytes[1] + 256 * (long)bytes[2] + (long)bytes[3]);
            return QueryDb(convertedIp, ipStr);
        }

        /// <summary>
        /// Queries the database for the country information.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="ipStr">The ip as a string.</param>
        /// <returns>The server's country information as a <see cref="CountryData"/> object.</returns>
        private CountryData QueryDb(long ip, string ipStr)
        {
            CountryData cd = null;
            try
            {
                //using (var cmd = new SQLiteCommand(_conn))
                using (var cmd = new Mono.Data.Sqlite.SqliteCommand(_conn))
                {
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.CommandText =
                        "SELECT name, country FROM countries WHERE begin_num <= @ip AND end_num >= @ip LIMIT 1";

                    //using (SQLiteDataReader reader = cmd.ExecuteReader())
                    using (Mono.Data.Sqlite.SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            cd = SetUnknownCountryData();
                            LoggerUtil.LogInfoAndDebug(string.Format(
                                "Country not found for: {0}, {1}.", ipStr, ip), LogClassType);
                        }
                        while (reader.Read())
                        {
                            cd = new CountryData
                            {
                                countryName = (string)reader["name"],
                                countryCode = (string)reader["country"],
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cd = SetUnknownCountryData();
                LoggerUtil.LogInfoAndDebug(string.Format("Country retrieval: problem looking up IP in database. Exception: " + ex),
                    LogClassType);
            }
            return cd;
        }

        /// <summary>
        /// Sets the country data to unknown when it cannot be found for whatever reason.
        /// </summary>
        /// <returns>The server's country information as a <see cref="CountryData"/> object with default 'unknown' values.</returns>
        private CountryData SetUnknownCountryData()
        {
            return new CountryData
            {
                countryName = "Unknown",
                countryCode = "Unknown",
            };
        }
    }
}