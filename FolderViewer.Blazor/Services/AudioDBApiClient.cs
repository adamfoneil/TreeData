using Refit;
using System.Threading.Tasks;

namespace FolderViewer.Blazor.Services
{
    public class AudioDBApiClient
    {
        private readonly IAudioDBApi _api = null;

        public AudioDBApiClient()
        {
            _api = RestService.For<IAudioDBApi>("https://www.theaudiodb.com");
        }

        public async Task<Artist[]> SearchArtistsAsync(string query) => (await _api.SearchArtistsAsync(query)).Artists;

        public async Task<Album[]> SearchOneAlbumAsync(string artist, string album) => (await _api.SearchAlbumsAsync(artist, album)).Album;
    }

    internal interface IAudioDBApi
    {
        [Get("/api/v1/json/1/search.php?s={query}")]
        Task<ArtistsResult> SearchArtistsAsync(string query);

        [Get("/api/v1/json/1/searchalbum.php?s={artist}&a={album}")]
        Task<AlbumResult> SearchAlbumsAsync(string artist, string album);
    }

    public class ArtistsResult
    {
        public Artist[] Artists { get; set; }
    }

    public class Artist
    {
        public string idArtist { get; set; }
        public string strArtist { get; set; }
        public object strArtistStripped { get; set; }
        public string strArtistAlternate { get; set; }
        public object strLabel { get; set; }
        public object idLabel { get; set; }
        public string intFormedYear { get; set; }
        public object intBornYear { get; set; }
        public string intDiedYear { get; set; }
        public string strDisbanded { get; set; }
        public string strStyle { get; set; }
        public string strGenre { get; set; }
        public string strMood { get; set; }
        public string strWebsite { get; set; }
        public string strFacebook { get; set; }
        public string strTwitter { get; set; }
        public string strBiographyEN { get; set; }
        public string strBiographyDE { get; set; }
        public string strBiographyFR { get; set; }
        public object strBiographyCN { get; set; }
        public object strBiographyIT { get; set; }
        public object strBiographyJP { get; set; }
        public object strBiographyRU { get; set; }
        public string strBiographyES { get; set; }
        public string strBiographyPT { get; set; }
        public object strBiographySE { get; set; }
        public object strBiographyNL { get; set; }
        public object strBiographyHU { get; set; }
        public object strBiographyNO { get; set; }
        public object strBiographyIL { get; set; }
        public string strBiographyPL { get; set; }
        public string strGender { get; set; }
        public string intMembers { get; set; }
        public string strCountry { get; set; }
        public string strCountryCode { get; set; }
        public string strArtistThumb { get; set; }
        public string strArtistLogo { get; set; }
        public object strArtistClearart { get; set; }
        public string strArtistWideThumb { get; set; }
        public string strArtistFanart { get; set; }
        public string strArtistFanart2 { get; set; }
        public string strArtistFanart3 { get; set; }
        public string strArtistBanner { get; set; }
        public string strMusicBrainzID { get; set; }
        public string strLastFMChart { get; set; }
        public string intCharted { get; set; }
        public string strLocked { get; set; }
    }

    public class AlbumResult
    {
        public Album[] Album { get; set; }
    }

    public class Album
    {
        public string idAlbum { get; set; }
        public string idArtist { get; set; }
        public string idLabel { get; set; }
        public string strAlbum { get; set; }
        public string strAlbumStripped { get; set; }
        public string strArtist { get; set; }
        public string strArtistStripped { get; set; }
        public string intYearReleased { get; set; }
        public string strStyle { get; set; }
        public string strGenre { get; set; }
        public string strLabel { get; set; }
        public string strReleaseFormat { get; set; }
        public string intSales { get; set; }
        public string strAlbumThumb { get; set; }
        public object strAlbumThumbHQ { get; set; }
        public string strAlbumThumbBack { get; set; }
        public string strAlbumCDart { get; set; }
        public string strAlbumSpine { get; set; }
        public string strAlbum3DCase { get; set; }
        public string strAlbum3DFlat { get; set; }
        public string strAlbum3DFace { get; set; }
        public object strAlbum3DThumb { get; set; }
        public string strDescriptionEN { get; set; }
        public object strDescriptionDE { get; set; }
        public string strDescriptionFR { get; set; }
        public object strDescriptionCN { get; set; }
        public object strDescriptionIT { get; set; }
        public object strDescriptionJP { get; set; }
        public object strDescriptionRU { get; set; }
        public string strDescriptionES { get; set; }
        public string strDescriptionPT { get; set; }
        public object strDescriptionSE { get; set; }
        public object strDescriptionNL { get; set; }
        public object strDescriptionHU { get; set; }
        public object strDescriptionNO { get; set; }
        public object strDescriptionIL { get; set; }
        public object strDescriptionPL { get; set; }
        public string intLoved { get; set; }
        public string intScore { get; set; }
        public string intScoreVotes { get; set; }
        public string strReview { get; set; }
        public string strMood { get; set; }
        public object strTheme { get; set; }
        public string strSpeed { get; set; }
        public object strLocation { get; set; }
        public string strMusicBrainzID { get; set; }
        public string strMusicBrainzArtistID { get; set; }
        public string strAllMusicID { get; set; }
        public string strBBCReviewID { get; set; }
        public string strRateYourMusicID { get; set; }
        public string strDiscogsID { get; set; }
        public string strWikidataID { get; set; }
        public string strWikipediaID { get; set; }
        public object strGeniusID { get; set; }
        public object strLyricWikiID { get; set; }
        public string strMusicMozID { get; set; }
        public object strItunesID { get; set; }
        public object strAmazonID { get; set; }
        public string strLocked { get; set; }
    }

}
