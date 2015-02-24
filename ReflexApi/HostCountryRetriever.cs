using System;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Reflection;
using ReflexAPI.SteamData;
using ReflexAPI.Util;

namespace ReflexAPI
{
    /// <summary>
    /// Class for retrieving country information from SQLite DB.
    /// </summary>
    public class HostCountryRetriever
    {
        private static readonly Type LogClassType = MethodBase.GetCurrentMethod().DeclaringType;

        private readonly Mono.Data.Sqlite.SqliteConnection _monoSqlConn;

        private readonly string _sqlConString = "Data Source=" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
           "countries.sqlite");

        private readonly SQLiteConnection _winSqlConn;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostCountryRetriever"/> class.
        /// </summary>
        public HostCountryRetriever()
        {
            // See which type of connection we need to open, based on runtime environment (linux/windows)
            if (IsRunningOnMono())
            {
                _monoSqlConn = new Mono.Data.Sqlite.SqliteConnection(_sqlConString);
                _monoSqlConn.Open();
            }
            else
            {
                _winSqlConn = new SQLiteConnection(_sqlConString);
                _winSqlConn.Open();
            }
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

            return IsRunningOnMono() ? QueryDb(new Mono.Data.Sqlite.SqliteCommand(_monoSqlConn), convertedIp, ipStr) :
                QueryDb(new SQLiteCommand(_winSqlConn), convertedIp, ipStr);
        }

        /// <summary>
        /// Determines whether ReflexAPI is running on Mono.
        /// </summary>
        /// <returns><c>true</c> if ReflexAPI is running on Mono,
        ///  otherwise <c>false</c> (running on Windows)</returns>
        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        /// Queries the database for the country information.
        /// </summary>
        /// <param name="cmd">The SqliteCommand.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="ipStr">The ip as a string.</param>
        /// <returns>
        /// The server's country information as a <see cref="CountryData" /> object.
        /// </returns>
        /// <remarks>
        /// This method is for Mono (linux) compatibility.
        /// </remarks>
        private CountryData QueryDb(Mono.Data.Sqlite.SqliteCommand cmd, long ip, string ipStr)
        {
            CountryData cd = null;
            try
            {
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.CommandText =
                        "SELECT name, country FROM countries WHERE begin_num <= @ip AND end_num >= @ip LIMIT 1";

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
        /// Queries the database for the country information.
        /// </summary>
        /// <param name="cmd">The SQLiteCommand.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="ipStr">The ip as a string.</param>
        /// <returns>
        /// The server's country information as a <see cref="CountryData" /> object.
        /// </returns>
        private CountryData QueryDb(SQLiteCommand cmd, long ip, string ipStr)
        {
            CountryData cd = null;
            try
            {
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.CommandText =
                        "SELECT name, country FROM countries WHERE begin_num <= @ip AND end_num >= @ip LIMIT 1";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
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