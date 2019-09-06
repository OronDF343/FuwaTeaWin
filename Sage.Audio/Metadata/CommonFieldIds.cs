using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sage.Audio.Metadata
{
    public static class CommonFieldIds
    {
		public static string Title { get; } = nameof(Title).ToUpperInvariant();
		public static string TitleSort { get; } = nameof(TitleSort).ToUpperInvariant();
		public static string Artist { get; } = nameof(Artist).ToUpperInvariant();
		public static string ArtistSort { get; } = nameof(ArtistSort).ToUpperInvariant();
		public static string Album { get; } = nameof(Album).ToUpperInvariant();
		public static string AlbumSort { get; } = nameof(AlbumSort).ToUpperInvariant();
		public static string Year { get; } = nameof(Year).ToUpperInvariant();
		public static string Track { get; } = nameof(Track).ToUpperInvariant();
		public static string TrackCount { get; } = nameof(TrackCount).ToUpperInvariant();
		public static string Genre { get; } = nameof(Genre).ToUpperInvariant();
		public static string Comment { get; } = nameof(Comment).ToUpperInvariant();
		public static string AlbumArtist { get; } = nameof(AlbumArtist).ToUpperInvariant();
		public static string AlbumArtistSort { get; } = nameof(AlbumArtistSort).ToUpperInvariant();
		public static string Composer { get; } = nameof(Composer).ToUpperInvariant();
		public static string ComposerSort { get; } = nameof(ComposerSort).ToUpperInvariant();
		public static string Disc { get; } = nameof(Disc).ToUpperInvariant();
		public static string DiscCount { get; } = nameof(DiscCount).ToUpperInvariant();
		public static string Bpm { get; } = nameof(Bpm).ToUpperInvariant();
		public static string Conductor { get; } = nameof(Conductor).ToUpperInvariant();
		public static string ContentGroup { get; } = nameof(ContentGroup).ToUpperInvariant();
		public static string Copyright { get; } = nameof(Copyright).ToUpperInvariant();
		public static string EncodedBy { get; } = nameof(EncodedBy).ToUpperInvariant();
		public static string EncoderSettings { get; } = nameof(EncoderSettings).ToUpperInvariant();
		public static string EncodingTime { get; } = nameof(EncodingTime).ToUpperInvariant();
		public static string FileOwner { get; } = nameof(FileOwner).ToUpperInvariant();
		public static string Grouping { get; } = nameof(Grouping).ToUpperInvariant();
		public static string InitialKey { get; } = nameof(InitialKey).ToUpperInvariant();
		public static string InvolvedPeople { get; } = nameof(InvolvedPeople).ToUpperInvariant();
		public static string Isrc { get; } = nameof(Isrc).ToUpperInvariant();
		public static string Language { get; } = nameof(Language).ToUpperInvariant();
		public static string Length { get; } = nameof(Length).ToUpperInvariant();
		public static string Lyricist { get; } = nameof(Lyricist).ToUpperInvariant();
		public static string MediaType { get; } = nameof(MediaType).ToUpperInvariant();
		public static string MixArtist { get; } = nameof(MixArtist).ToUpperInvariant();
		public static string Mood { get; } = nameof(Mood).ToUpperInvariant();
		public static string MovementName { get; } = nameof(MovementName).ToUpperInvariant();
		public static string Movement { get; } = nameof(Movement).ToUpperInvariant();
		public static string MovementTotal { get; } = nameof(MovementTotal).ToUpperInvariant();
		public static string MusicianCredits { get; } = nameof(MusicianCredits).ToUpperInvariant();
		public static string NetRadioOwner { get; } = nameof(NetRadioOwner).ToUpperInvariant();
		public static string NetRadioStation { get; } = nameof(NetRadioStation).ToUpperInvariant();
		public static string OrigAlbum { get; } = nameof(OrigAlbum).ToUpperInvariant();
		public static string OrigArtist { get; } = nameof(OrigArtist).ToUpperInvariant();
		public static string OrigFileName { get; } = nameof(OrigFileName).ToUpperInvariant();
		public static string OrigLyricist { get; } = nameof(OrigLyricist).ToUpperInvariant();
		public static string OrigReleaseTime { get; } = nameof(OrigReleaseTime).ToUpperInvariant();
		public static string Publisher { get; } = nameof(Publisher).ToUpperInvariant();
		public static string Rating_Mm { get; } = nameof(Rating_Mm).ToUpperInvariant();
		public static string Rating_WinAmp { get; } = nameof(Rating_WinAmp).ToUpperInvariant();
		public static string Rating_Wmp { get; } = nameof(Rating_Wmp).ToUpperInvariant();
		public static string ReleaseTime { get; } = nameof(ReleaseTime).ToUpperInvariant();
		public static string SetSubtitle { get; } = nameof(SetSubtitle).ToUpperInvariant();
		public static string Subtitle { get; } = nameof(Subtitle).ToUpperInvariant();
		public static string TaggingTime { get; } = nameof(TaggingTime).ToUpperInvariant();
		public static string UnsyncedLyrics { get; } = nameof(UnsyncedLyrics).ToUpperInvariant();
		public static string WwwArtist { get; } = nameof(WwwArtist).ToUpperInvariant();
		public static string WwwAudioFile { get; } = nameof(WwwAudioFile).ToUpperInvariant();
		public static string WwwAudioSource { get; } = nameof(WwwAudioSource).ToUpperInvariant();
		public static string WwwCommercialInfo { get; } = nameof(WwwCommercialInfo).ToUpperInvariant();
		public static string WwwCopyright { get; } = nameof(WwwCopyright).ToUpperInvariant();
		public static string WwwPayment { get; } = nameof(WwwPayment).ToUpperInvariant();
		public static string WwwPublisher { get; } = nameof(WwwPublisher).ToUpperInvariant();
		public static string WwwRadioPage { get; } = nameof(WwwRadioPage).ToUpperInvariant();
		public static string Encoder { get; } = nameof(Encoder).ToUpperInvariant();
		
		public static IReadOnlyList<string> All { get; } = new ReadOnlyCollection<string>(new List<string>
		{
			Title,
			TitleSort,
			Artist,
			ArtistSort,
			Album,
			AlbumSort,
			Year,
			Track,
			TrackCount,
			Genre,
			Comment,
			AlbumArtist,
			AlbumArtistSort,
			Composer,
			ComposerSort,
			Disc,
			DiscCount,
			Bpm,
			Conductor,
			ContentGroup,
			Copyright,
			EncodedBy,
			EncoderSettings,
			EncodingTime,
			FileOwner,
			Grouping,
			InitialKey,
			InvolvedPeople,
			Isrc,
			Language,
			Length,
			Lyricist,
			MediaType,
			MixArtist,
			Mood,
			MovementName,
			Movement,
			MovementTotal,
			MusicianCredits,
			NetRadioOwner,
			NetRadioStation,
			OrigAlbum,
			OrigArtist,
			OrigFileName,
			OrigLyricist,
			OrigReleaseTime,
			Publisher,
			Rating_Mm,
			Rating_WinAmp,
			Rating_Wmp,
			ReleaseTime,
			SetSubtitle,
			Subtitle,
			TaggingTime,
			UnsyncedLyrics,
			WwwArtist,
			WwwAudioFile,
			WwwAudioSource,
			WwwCommercialInfo,
			WwwCopyright,
			WwwPayment,
			WwwPublisher,
			WwwRadioPage,
			Encoder,
		});

		public static IReadOnlyDictionary<string, string> Id3V2ToFieldId { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
		{
			{ "TIT2" , Title },
			{ "TSOT" , TitleSort },
			{ "TPE1" , Artist },
			{ "TSOP" , ArtistSort },
			{ "TALB" , Album },
			{ "TSOA" , AlbumSort },
			{ "TDRC" , Year },
			{ "TRCK" , Track },
			{ "TCON" , Genre },
			{ "COMM" , Comment },
			{ "TPE2" , AlbumArtist },
			{ "TSO2" , AlbumArtistSort },
			{ "TCOM" , Composer },
			{ "TSOC" , ComposerSort },
			{ "TPOS" , Disc },
			{ "TBPM" , Bpm },
			{ "TPE3" , Conductor },
			{ "TIT1" , ContentGroup },
			{ "TCOP" , Copyright },
			{ "TENC" , EncodedBy },
			{ "TSSE" , EncoderSettings },
			{ "TDEN" , EncodingTime },
			{ "TOWN" , FileOwner },
			{ "GRP1" , Grouping },
			{ "TKEY" , InitialKey },
			{ "TIPL" , InvolvedPeople },
			{ "TSRC" , Isrc },
			{ "TLAN" , Language },
			{ "TLEN" , Length },
			{ "TEXT" , Lyricist },
			{ "TMED" , MediaType },
			{ "TPE4" , MixArtist },
			{ "TMOO" , Mood },
			{ "MVNM" , MovementName },
			{ "MVIN" , Movement },
			{ "TMCL" , MusicianCredits },
			{ "TRSO" , NetRadioOwner },
			{ "TRSN" , NetRadioStation },
			{ "TOAL" , OrigAlbum },
			{ "TOPE" , OrigArtist },
			{ "TOFN" , OrigFileName },
			{ "TOLY" , OrigLyricist },
			{ "TDOR" , OrigReleaseTime },
			{ "TPUB" , Publisher },
			{ "TDRL" , ReleaseTime },
			{ "TSST" , SetSubtitle },
			{ "TIT3" , Subtitle },
			{ "TDTG" , TaggingTime },
			{ "USLT" , UnsyncedLyrics },
			{ "WOAR" , WwwArtist },
			{ "WOAF" , WwwAudioFile },
			{ "WOAS" , WwwAudioSource },
			{ "WCOM" , WwwCommercialInfo },
			{ "WCOP" , WwwCopyright },
			{ "WPAY" , WwwPayment },
			{ "WPUB" , WwwPublisher },
			{ "WORS" , WwwRadioPage },
			{ "TYER" , Year },
			{ "TDAT" , Year },
			{ "TIME" , Year },
			{ "IPLS" , InvolvedPeople },
			{ "TORY" , OrigReleaseTime },
		});

		public static IReadOnlyDictionary<string, string> AppleToFieldId { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
		{
			{ "\u00A9alb" , Album },
			{ "\u00A9art" , Artist },
			{ "aART" , AlbumArtist },
			{ "\u00A9cmt" , Comment },
			{ "\u00A9day" , Year },
			{ "\u00A9nam" , Title },
			{ "\u00A9gen" , Genre },
			{ "gnre" , Genre },
			{ "trkn" , Track },
			{ "disk" , Disc },
			{ "\u00A9wrt" , Composer },
			{ "\u00A9too" , Encoder },
			{ "tmpo" , Bpm },
			{ "cprt" , Copyright },
			{ "\u00A9grp" , ContentGroup },
			{ "\u00A9lyr" , UnsyncedLyrics },
			{ "soal" , AlbumSort },
			{ "soaa" , AlbumArtistSort },
			{ "soar" , ArtistSort },
			{ "soco" , ComposerSort },
			{ "\u00A9con" , Conductor },
			{ "\u00A9mvn" , MovementName },
			{ "\u00A9mvi" , Movement },
			{ "\u00A9mvc" , MovementTotal },
			{ "sonm" , TitleSort },
		});
    }
}
