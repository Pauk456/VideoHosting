namespace GatewayService.Controllers
{
    public static class ServicesAddresses
    {
        public static readonly string uriTitleService = "http://titleservice:5006/api/title";
        public static readonly string uriServerInteraction = "http://host.docker.internal:4999";
        public static readonly string uriRecommendationService = "http://titlerecommendationservice:5007/TitleRecommendation";
        public static readonly string uriSearchService = "http://searchservice:5003/Search";
        public static readonly string uriVideoAndImageService = "http://videoservice:5001/Search/api/img";
        public static readonly string uriGatewayLocal = "http://localhost:5004/api";
        public static readonly string uriVideoService = "http://videoservice:5001/Search/api/video";
    }
}
