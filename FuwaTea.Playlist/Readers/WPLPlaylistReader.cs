using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using FuwaTea.Lib;
using FuwaTea.Lib.Collections;
using FuwaTea.Lib.Exceptions;
using log4net;

namespace FuwaTea.Playlist.Readers
{
    [PlaylistHandler("WPL Playlist Reader")]
    public class WPLPlaylistReader : IPlaylistReader
    {
        public IEnumerable<string> SupportedFileTypes => new[] {"wpl|Windows Media Playlist"};

        public void LoadPlaylistFiles(string path, IPlaylist playlist)
        {
            var log = LogManager.GetLogger(GetType());

            // Load file
            var xd = XDocument.Load(path);
            // Check for this: <?wpl version="1.0"?>
            var xpi = (XProcessingInstruction)xd.Nodes().FirstOrDefault(xn => xn.NodeType == XmlNodeType.ProcessingInstruction
                                                                              && ((XProcessingInstruction)xn).Target == "wpl");
            if (xpi == null) log.Warn("Missing WPL XProcessingInstruction!");
            else if (xpi.Data.Replace(" ", "") != "version=\"1.0\"")
                log.Warn("Incorrect WPL Version!");
            // Get element: <smil>
            var smil = (XElement)xd.Nodes().FirstOrDefault(xn => xn.NodeType == XmlNodeType.Element
                                                                 && ((XElement)xn).Name.LocalName == "smil");
            if (smil == null) throw new DataSourceException(path, "Missing element: smil"); // TODO: ErrorCallback
            // Get element: <head>
            var head = smil.Elements().FirstOrDefault(xe => xe.Name.LocalName == "head");
            if (head == null) log.Info("Missing element: head");
            else
            {
                var title = head.Descendants(XName.Get("title")).FirstOrDefault();
                if (title == null) log.Debug("Missing element: title");
                else playlist.Title = title.Value;
                foreach (var meta in head.Descendants(XName.Get("meta")))
                {
                    try
                    {
                        playlist.Metadata.AddOrSet(meta.Attribute(XName.Get("name")).Value,
                                                   meta.Attribute(XName.Get("content")).Value);
                    }
                    catch (Exception e)
                    {
                        log.Warn("Invalid element: meta", e);
                    }
                }
            }
            // Get elements: <body><seq>
            var body = smil.Elements().FirstOrDefault(xe => xe.Name.LocalName == "body");
            if (body == null) throw new DataSourceException(path, "Missing element: body"); // TODO: ErrorCallback
            var seq = body.Elements().FirstOrDefault(xe => xe.Name.LocalName == "seq");
            if (seq == null) throw new DataSourceException(path, "Missing element: seq"); // TODO: ErrorCallback
            // Show error if fount: <smartPlaylist>
            if (seq.Descendants().Any(xe => xe.Name.LocalName == "smartPlaylist"))
                log.Error("smartPlaylist not supported!"); // TODO: ErrorCallback
            // Get all: <media>
            var files = new List<string>();
            foreach (var media in seq.Descendants(XName.Get("media")))
            {
                try
                {
                    files.Add(PathUtils.ExpandRelativePath(Path.GetDirectoryName(path), media.Attribute(XName.Get("src")).Value));
                }
                catch (Exception e)
                {
                    log.Warn("Invalid element: media", e);
                }
            }
            // Init playlist
            playlist.Init(path, files);
        }
    }
}
