using System;
using System.Collections.Generic;

namespace FuwaTea.Audio.Metadata
{
    public interface IMetadata
    {
        // Used mostly for streams
        bool IsReadOnly { get; }
        // 
        bool SupportsUnicode { get; }

        // ID3v2.3 TXXX
        // ID3v2.3 WXXX (ISO-8859-1 only)
        IDictionary<string, IList<string>> ExtendedFields { get; }
        // Special field / not part of tags
        // ID3v2.3 APIC
        /* This frame contains a picture directly related to the audio file. Image format is the MIME type and subtype for
         * the image. In the event that the MIME media type name is omitted, "image/" will be implied. The "image/png" or
         * "image/jpeg" picture format should be used when interoperability is wanted. Description is a short description
         * of the picture, represented as a terminated text string. The description has a maximum length of 64 characters,
         * but may be empty. There may be several pictures attached to one file, each in their individual "APIC" frame,
         * but only one with the same content descriptor. There may only be one picture with the picture type declared as
         * picture type $01 and $02 respectively. There is the possibility to put only a link to the image file by using
         * the 'MIME type' "-->" and having a complete URL instead of picture data. The use of linked files should however
         * be used sparingly since there is the risk of separation of files. 
         */
        IList<IPicture> Picture { get; }

        // Mp3tag basic tags
        // ID3v2.3 TIT2
        ITextField Title { get; }
        // ID3v2.4 TSOT ([UNOFFICIAL] ID3v2.3)
        ITextField TitleSort { get; }
        // ID3v2.3 TPE1 (separator /)
        IListField Artist { get; }
        // ID3v2.4 TSOP ([UNOFFICIAL] ID3v2.3)
        IListField ArtistSort { get; }
        // ID3v2.3 TALB
        ITextField Album { get; }
        // ID3v2.4 TSOA ([UNOFFICIAL] ID3v2.3)
        ITextField AlbumSort { get; }
        // ID3v2.3 TYER (year, numeric 4-char) + TDAT + TIME
        // TDAT: The 'Date' frame is a numeric string in the DDMM format containing the date for the recording. This field is always four characters long.
        // TIME: The 'Time' frame is a numeric string in the HHMM format containing the time for the recording. This field is always four characters long.
        // ID3v2.4 TDRC (timestamp)
        IDateTimeField Year { get; }
        // ID3v2.3 TRCK (divider /)
        INumericField Track { get; }
        //     [As above]
        INumericField TrackCount { get; }
        // ID3v2.3 TCON:
        /* The 'Content type', which previously was stored as a one byte numeric value only, is now a numeric string.
         * You may use one or several of the types as ID3v1.1 did or, since the category list would be impossible to
         * maintain with accurate and up to date categories, define your own.
         * References to the ID3v1 genres can be made by, as first byte, enter "(" followed by a number from the
         * genres list (appendix A) and ended with a ")" character. This is optionally followed by a refinement, e.g.
         * "(21)" or "(4)Eurodisco". Several references can be made in the same frame, e.g. "(51)(39)". If the
         * refinement should begin with a "(" character it should be replaced with "((", e.g. "((I can figure out any
         * genre)" or "(55)((I think...)". The following new content types is defined in ID3v2 and is implemented in
         * the same way as the numeric content types, e.g. "(RX)".
         * RX    Remix
         * CR    Cover
         */
        IListField Genre { get; }
        // ID3v2.3 COMM
        /* This frame is intended for any kind of full text information that does not fit in any other frame. It
         * consists of a frame header followed by encoding, language and content descriptors and is ended with the
         * actual comment as a text string. Newline characters are allowed in the comment text string. There may be more
         * than one comment frame in each tag, but only one with the same language and content descriptor. 
         */
        IListField Comment { get; }
        // ID3v2.3 TPE2
        ITextField AlbumArtist { get; }
        // [UNOFFICIAL] ID3v2.3/4 TSO2
        ITextField AlbumArtistSort { get; }
        // ID3v2.3 TCOM (separator /)
        IListField Composer { get; }
        // ID3v2.4 TSOC ([UNOFFICIAL] ID3v2.3)
        IListField ComposerSort { get; }
        // ID3v2.3 TPOS (divider /)
        INumericField Disc { get; }
        //     [As above]
        INumericField DiscCount { get; }

        // Mp3tag extended tags
        // ID3v2.3 TBPM
        INumericField Bpm { get; }
        // ID3v2.3 TPE3
        ITextField Conductor { get; }
        // ID3v2.3 TIT1
        ITextField ContentGroup { get; }
        // ID3v2.3 TCOP
        /* The 'Copyright message' frame, which must begin with a year and a space character (making five characters),
         * is intended for the copyright holder of the original sound, not the audio file itself. The absence of this
         * frame means only that the copyright information is unavailable or has been removed, and must not be
         * interpreted to mean that the sound is public domain. Every time this field is displayed the field must be
         * preceded with "Copyright © ".
         */
        ITextField Copyright { get; }
        // ID3v2.3 TENC
        ITextField EncodedBy { get; }
        // ID3v2.3 TSSE
        ITextField EncoderSettings { get; }
        // ID3v2.4 TDEN (timestamp)
        IDateTimeField EncodingTime { get; }
        // ID3v2.3 TOWN
        ITextField FileOwner { get; }
        // [UNOFFICIAL] ID3v2.3 GRP1
        ITextField Grouping { get; }
        // ID3v2.3 TKEY
        /* The 'Initial key' frame contains the musical key in which the sound starts. It is represented as a string with a
         maximum length of three characters. The ground keys are represented with "A","B","C","D","E", "F" and "G" and halfkeys
         represented with "b" and "#". Minor is represented as "m". Example "Cbm". Off key is represented with an "o" only.
         */
        IEnumField<MusicalKey> InitialKey { get; }
        // ID3v2.3 IPLS
        /* Since there might be a lot of people contributing to an audio file in various ways, such as musicians and technicians,
         * the 'Text information frames' are often insufficient to list everyone involved in a project. The 'Involved people list'
         * is a frame containing the names of those involved, and how they were involved. The body simply contains a terminated
         * string with the involvement directly followed by a terminated string with the involvee followed by a new involvement
         * and so on. There may only be one "IPLS" frame in each tag. 
         */
        // ID3v2.4 TIPL
        /* The 'Involved people list' is very similar to the musician credits
         * list, but maps between functions, like producer, and names.
         */
        IListField InvolvedPeople { get; }
        // ID3v2.3 TSRC
        ITextField Isrc { get; }
        // ID3v2.3 TLAN
        /* The 'Language(s)' frame should contain the languages of the text or lyrics spoken or sung in the audio. The language
         * is represented with three characters according to ISO-639-2. If more than one language is used in the text their
         * language codes should follow according to their usage. 
         */
        ITextField Language { get; }
        // ID3v2.3 TLEN
        // The 'Length' frame contains the length of the audiofile in milliseconds, represented as a numeric string. 
        INumericField Length { get; }
        // ID3v2.3 TEXT (separator /)
        IListField Lyricist { get; }
        // ID3v2.3 TMED
        /* The 'Media type' frame describes from which media the sound originated. This may be a text string or a reference to
         * the predefined media types found in the list below. References are made within "(" and ")" and are optionally followed
         * by a text refinement, e.g. "(MC) with four channels". If a text refinement should begin with a "(" character it should
         * be replaced with "((" in the same way as in the "TCO" frame. Predefined refinements is appended after the media type,
         * e.g. "(CD/A)" or "(VID/PAL/VHS)".
         * DIG     Other digital media
         *     /A  Analog transfer from media
         * ANA     Other analog media
         *     /WAC Wax cylinder
         *     /8CA 8-track tape cassette
         * CD      CD
         *     /A Analog transfer from media
         *     /DD DDD
         *     /AD ADD
         *     /AA AAD
         * LD      Laserdisc
         *     /A Analog transfer from media
         * TT      Turntable records
         *     /33 33.33 rpm
         *     /45 45 rpm
         *     /71 71.29 rpm
         *     /76 76.59 rpm
         *     /78 78.26 rpm
         *     /80 80 rpm
         * MD      MiniDisc
         *     /A Analog transfer from media
         * DAT     DAT
         *     /A Analog transfer from media
         *     /1 standard, 48 kHz/16 bits, linear
         *     /2 mode 2, 32 kHz/16 bits, linear
         *     /3 mode 3, 32 kHz/12 bits, nonlinear, low speed
         *     /4 mode 4, 32 kHz/12 bits, 4 channels
         *     /5 mode 5, 44.1 kHz/16 bits, linear
         *     /6 mode 6, 44.1 kHz/16 bits, 'wide track' play
         * DCC     DCC
         *     /A Analog transfer from media
         * DVD     DVD
         *     /A Analog transfer from media
         * TV      Television
         *     /PAL PAL
         *     /NTSC NTSC
         *     /SECAM SECAM
         * VID     Video
         *     /PAL PAL
         *     /NTSC NTSC
         *     /SECAM SECAM
         *     /VHS VHS
         *     /SVHS S-VHS
         *     /BETA BETAMAX
         * RAD     Radio
         *     /FM FM
         *     /AM AM
         *     /LW LW
         *     /MW MW
         * TEL     Telephone
         *     /I ISDN
         * MC      MC (normal cassette)
         *     /4 4.75 cm/s (normal speed for a two sided cassette)
         *     /9 9.5 cm/s
         *     /I Type I cassette (ferric/normal)
         *     /II Type II cassette (chrome)
         *     /III Type III cassette (ferric chrome)
         *     /IV Type IV cassette (metal)
         * REE     Reel
         *     /9 9.5 cm/s
         *     /19 19 cm/s
         *     /38 38 cm/s
         *     /76 76 cm/s
         *     /I Type I cassette (ferric/normal)
         *     /II Type II cassette (chrome)
         *     /III Type III cassette (ferric chrome)
         *     /IV Type IV cassette (metal)
         */
        ITextField MediaType { get; }
        // ID3v2.3 TPE4
        ITextField MixArtist { get; }
        // ID3v2.4 TMOO
        ITextField Mood { get; }
        // [UNOFFICIAL] ID3v2.3 MVNM
        ITextField MovementName { get; }
        // [UNOFFICIAL] ID3v2.3 MVIN (separator /)
        INumericField Movement { get; }
        //     [As above]
        INumericField MovementTotal { get; }
        // ID3v2.4 TMCL
        /* The 'Musician credits list' is intended as a mapping between
         * instruments and the musician that played it. Every odd field is an
         * instrument and every even is an artist or a comma delimited list of
         * artists.
         */
        IListField MusicianCredits { get; }
        // ID3v2.3 TRSO
        ITextField NetRadioOwner { get; }
        // ID3v2.3 TRSN
        ITextField NetRadioStation { get; }
        // ID3v2.3 TOAL
        ITextField OrigAlbum { get; }
        // ID3v2.3 TOPE
        ITextField OrigArtist { get; }
        // ID3v2.3 TOFN
        ITextField OrigFileName { get; }
        // ID3v2.3 TOLY
        ITextField OrigLyricist { get; }
        // ID3v2.3 TORY (year)
        // ID3v2.4 TDOR (timestamp)
        IDateTimeField OrigReleaseTime { get; }
        // ID3v2.3 TPUB
        ITextField Publisher { get; }
        // ID3v2.3 POPM
        // Popularimeter. Stores 1-byte "rating" field - 0 = unknown, 1 = worst, 255 = best. Has a play counter (not to be confused with PCNT)
        // MediaMonkey: 5 stars = 255, 4 stars = 196, 3 stars = 128, 2 stars = 64, 1 star = 1
        INumericField Rating_Mm { get; }
        // WinAmp: 5 stars = 255, 4 stars = 196, 3 stars = 128, 2 stars = 64, 1 star = 1
        INumericField Rating_WinAmp { get; }
        // WMP: 5 stars = 255, 4 stars = 196, 3 stars = 128, 2 stars = 64, 1 star = 1
        INumericField Rating_Wmp { get; }
        // ID3v2.4 TDRL (Mp3tag claims this is in ID3v2.3)
        /* The 'Release time' frame contains a timestamp describing when the
         * audio was first released. Timestamp format is described in the ID3v2
         * structure document [ID3v2-strct].
         */
        IDateTimeField ReleaseTime { get; }
        // ID3v2.4 TSST
        ITextField SetSubtitle { get; }
        // ID3v2.3 TIT3
        ITextField Subtitle { get; }
        // ID3v2.4 TDTG (timestamp)
        IDateTimeField TaggingTime { get; }
        // ID3v2.3 USLT
        /* This frame contains the lyrics of the song or a text transcription of other vocal activities. The head includes an
         * encoding descriptor and a content descriptor. The body consists of the actual text. The 'Content descriptor' is a
         * terminated string. If no descriptor is entered, 'Content descriptor' is $00 (00) only. Newline characters are allowed in
         * the text. There may be more than one 'Unsynchronised lyrics/text transcription' frame in each tag, but only one with the
         * same language and content descriptor. 
         */
        IListField UnSyncedLyrics { get; }
        // ID3v2.3 WOAR (more than one, distinct)
        IListField WwwArtist { get; }
        // ID3v2.3 WOAF
        ITextField WwwAudioFile { get; }
        // ID3v2.3 WOAS
        ITextField WwwAudioSource { get; }
        // ID3v2.3 WCOM (more than one, distinct)
        IListField WwwCommercialInfo { get; }
        // ID3v2.3 WCOP
        ITextField WwwCopyright { get; }
        // ID3v2.3 WPAY
        ITextField WwwPayment { get; }
        // ID3v2.3 WPUB
        ITextField WwwPublisher { get; }
        // ID3v2.3 WORS
        ITextField WwwRadioPage { get; }

        // Missing ID3v2.3 fields
        // TRDA: The 'Recording dates' frame is a intended to be used as complement to the "TYER", "TDAT" and "TIME" frames. E.g. "4th-7th June, 12th June" in combination with the "TYER" frame. [DEPRECATED]
        // TDLY: 'Playlist delay'. Not relevant to us
        // TSIZ: Size of the file. Useless to us [DEPRECATED]
        // MCDI: CD TOC binary dump, should not be editable
        // ETCO + MLLT: Timing, not relevant to us
        // SYTC + SYLT: Synchronized tempo and lyrics. Not currently supported
        // RVAD + EQUA + RVRB: User volume, EQ and reverb settings. Not supported. [DEPRECATED] except for RVRB
        // GEOB: Embedded file. Not supported
        // PCNT: Play counter. Not supported
        // RBUF: Recommended buffer size. Should be used by the stream decoder
        // AENC + ENCR: Audio / frame encryption (DRM?). Not supported
        // LINK: Linked info from other files. Not supported
        // POSS: Position sync for streaming. Stream decoder should look for this
        // USER: Terms of use
        // OWNE: The ownership frame might be used as a reminder of a made transaction or, if signed, as proof. Note that the "USER" and "TOWN" frames are good to use in conjunction with this one. The frame begins, after the frame ID, size and encoding fields, with a 'price payed' field. The first three characters of this field contains the currency used for the transaction, encoded according to ISO-4217 alphabetic currency code. Concatenated to this is the actual price payed, as a numerical string using "." as the decimal separator. Next is an 8 character date string (YYYYMMDD) followed by a string with the name of the seller as the last field in the frame. There may only be one "OWNE" frame in a tag. 
        // COMR: Sales info. Not supported
        // GRID: Frame grouping. Not supported
        // PRIV: Application-specific. Not supported
        // TFLT: File type. Not supported (useless)

        /* The timestamp fields are based on a subset of ISO 8601. When being as
           precise as possible the format of a time string is
           yyyy-MM-ddTHH:mm:ss (year, "-", month, "-", day, "T", hour (out of
           24), ":", minutes, ":", seconds), but the precision may be reduced by
           removing as many time indicators as wanted. Hence valid timestamps
           are
           yyyy, yyyy-MM, yyyy-MM-dd, yyyy-MM-ddTHH, yyyy-MM-ddTHH:mm and
           yyyy-MM-ddTHH:mm:ss. All time stamps are UTC. For durations, use
           the slash character as described in 8601, and for multiple non-
           contiguous dates, use multiple strings, if allowed by the frame
           definition.
         */

        // Missing ID3v2.4 fields
        // EQU2: Replaces EQUA. Not supported
        // RVA2: Replaces RVAD. Not supported
        // ASPI + SEEK: Not relevant to us
        // SIGN: Signature. Not supported
        /* TPRO
           The 'Produced notice' frame, in which the string must begin with a
           year and a space character (making five characters), is intended for
           the production copyright holder of the original sound, not the audio
           file itself. The absence of this frame means only that the production
           copyright information is unavailable or has been removed, and must
           not be interpreted to mean that the audio is public domain. Every
           time this field is displayed the field must be preceded with
           "Produced " (P) " ", where (P) is one character showing a P in a
           circle.
         */

        // Not supported by Vorbis Comment, not listed in Mp3tag
        // ID3v2.4 RVA2 ?
        //double ReplayGainTrackGain { get; }
        //double ReplayGainTrackPeak { get; }
        //double ReplayGainAlbumGain { get; }
        //double ReplayGainAlbumPeak { get; }
    }

    [Flags]
    public enum MusicalKey : short
    {
        OffKey =    0b00_00000000,
        C =         0b00_00000001,
        D =         0b00_00000010,
        E =         0b00_00000100,
        F =         0b00_00001000,
        G =         0b00_00010000,
        A =         0b00_00100000,
        B =         0b00_01000000,
        Sharp =     0b00_10000000,
        Flat =      0b01_00000000,
        Minor =     0b10_00000000
    }
}
