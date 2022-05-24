namespace GameOfRevenge.Common.Net
{
    public enum CaseType
    {
        /// <summary>
        /// 0-99
        /// </summary>
        Error,
        /// <summary>
        /// 100-199
        /// </summary>
        Success,
        /// <summary>
        /// 200-999
        /// </summary>
        Invalid,
        /// <summary>
        /// 999-9999
        /// </summary>
        Other
    }
}
