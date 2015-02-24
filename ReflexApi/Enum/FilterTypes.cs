namespace ReflexAPI.Enum
{
    /// <summary>
    /// Enum that represents the different types of Reflex server filters
    /// that a user can specify via the /servers URL API endpoint.
    /// </summary>
    public enum FilterTypes
    {
        None,
        ProtocolFilter,
        CountryCodeFilter,
        MapFilter,
        OsFilter,
        GameTypeFilter,
        PlayersTrueFilter,
        PlayersFalseFilter,
        NotFullTrueFilter,
        NotFullFalseFilter,
        PasswordFilter,
        VacFilter,
        VersionFilter
    }
}
