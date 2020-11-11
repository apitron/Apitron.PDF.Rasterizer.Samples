namespace PdfSearchSample
{
    using Apitron.PDF.Rasterizer.Search;

    internal struct ResultItem
    {
        public readonly SearchResultItem SearchResult;

        public readonly string FileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ResultItem( SearchResultItem searchResult, string fileName )
        {
            this.SearchResult = searchResult;
            this.FileName = fileName;
        }
    }
}