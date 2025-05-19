namespace GatewayService.Controllers
{
    public static class ServicesAddresses
    {
        public static readonly string uriTitleService = "http://titleservice:5006/api/title";
        public static readonly string uriServerInteraction = "http://host.docker.internal:4999";
        public static readonly string uriRecommendationService = "http://recommendationservice:5007/controller";
        public static readonly string uriSearchService = "http://searchservice:5003/Search";
    }
}
