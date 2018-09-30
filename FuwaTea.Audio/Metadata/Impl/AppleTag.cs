using System.Collections.Generic;

namespace FuwaTea.Audio.Metadata.Impl
{
    public class AppleTag : MetadataBase
    {
        public AppleTag(bool isReadOnly = false)
            : base(isReadOnly) { } // Forced UTF-8 without BOM

        // From http://atomicparsley.sourceforge.net/mpeg-4files.html

        public override ITextField Album { get; } = new BasicTextField(255);
        public override IListField Artist { get; } = new JoinedListField("; ", 255);
        public override ITextField AlbumArtist { get; } = new BasicTextField(255);
        public override IListField Comment { get; } = new BasicListField(1, 255);
        public override IDateTimeField Year { get; } = new BasicDateTimeField(); // No strict format
        public override ITextField Title { get; } = new BasicTextField(255);
        public override IListField Genre { get; } = new JoinedListField("; ", 255); // Supports either single ID or text
        public override INumericField Track { get; } = new BasicNumericField(255);
        public override INumericField Disc { get; } = new BasicNumericField(255);
        public override IListField Composer { get; } = new JoinedListField("; ", 255);
        public override ITextField EncodedBy { get; } = new BasicTextField(255);
        public override INumericField Bpm { get; } = new BasicNumericField(255);
        public override ITextField Copyright { get; } = new BasicTextField(255);
        public override IList<IPicture> Picture { get; } = new List<IPicture>(); // Only field that supports more than 1 value. JPEG or PNG. No description or type.
        public override ITextField Grouping { get; } = new BasicTextField(255);
        public override ITextField Subtitle { get; } = new BasicTextField(255); // Description
        public override IListField UnSyncedLyrics { get; } = new BasicListField(1); // Only text field that allows more than 255 characters

        // Supported by TagLib

        public override INumericField TrackCount { get; } = new BasicNumericField(255);
        public override INumericField DiscCount { get; } = new BasicNumericField(255);

        // Additional fields http://help.mp3tag.de/main_tags.html

        public override ITextField AlbumSort { get; } = new BasicTextField(255);
        public override ITextField AlbumArtistSort { get; } = new BasicTextField(255);
        public override IListField ArtistSort { get; } = new JoinedListField("; ", 255);
        public override IListField ComposerSort { get; } = new JoinedListField("; ", 255);
        public override ITextField Conductor { get; } = new BasicTextField(255);
        public override ITextField ContentGroup { get; } = new BasicTextField(255);
        public override ITextField MovementName { get; } = new BasicTextField(255);
        public override INumericField Movement { get; } = new BasicNumericField(255);
        public override INumericField MovementTotal { get; } = new BasicNumericField(255);
        public override ITextField TitleSort { get; } = new BasicTextField(255);
    }
}
