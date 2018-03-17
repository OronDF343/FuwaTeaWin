using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FuwaTea.Metadata.Tags;

namespace FuwaTea.Playback.NAudio.Utils
{
    // Based on code by Dirk Reske: http://www.codeproject.com/Articles/19125/ShoutcastStream-Class
    public class ShoutcastStream : Stream
    {
        private readonly int _metaInt;
        private int _receivedBytes;
        private readonly Stream _netStream;
        private bool _connected;
        
        public event EventHandler StreamTitleChanged;

        public NullTag StreamMetadata { get; }

        public ShoutcastStream(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Clear();
            request.Headers.Add("Icy-MetaData", "1");
            request.KeepAlive = false;
            request.UserAgent = "FuwaTeaWin/0.1";

            var response = (HttpWebResponse)request.GetResponse();
            _metaInt = int.Parse(response.Headers["Icy-MetaInt"]);
            StreamMetadata = new NullTag(int.Parse(response.Headers["icy-br"].Split(',')[0]), TimeSpan.Zero)
            {
                Performers = new[] { response.Headers["icy-name"] },
                Genres = response.Headers["icy-genre"].Split(' '),
                Comment = response.Headers["icy-notice1"] + "\n" + response.Headers["icy-notice2"] + "\n" + response.Headers["icy-url"]
            };
            MimeType = response.Headers["content-type"];
            _receivedBytes = 0;

            _netStream = response.GetResponseStream();
            _connected = true;
        }

        private void ParseMetaInfo(byte[] metaInfo)
        {
            var metaString = Encoding.ASCII.GetString(metaInfo);
            
            var newStreamTitle = Regex.Match(metaString, "(StreamTitle=')(.*)(';)").Groups[2].Value.Trim();
            if (newStreamTitle.Equals(StreamTitle)) return;
            StreamTitle = newStreamTitle;
            OnStreamTitleChanged();
        }

        protected virtual void OnStreamTitleChanged()
        {
            StreamTitleChanged?.Invoke(this, EventArgs.Empty);
        }

        public override bool CanRead => _connected;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public string StreamTitle { get => StreamMetadata.Title; private set => StreamMetadata.Title = value; }
        public string MimeType { get; }

        public override void Flush() { }
        
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                if (_receivedBytes == _metaInt)
                {
                    var metaLen = _netStream.ReadByte();
                    if (metaLen > 0)
                    {
                        var metaInfo = new byte[metaLen * 16];
                        var len = 0;
                        while ((len += _netStream.Read(metaInfo, len, metaInfo.Length - len)) < metaInfo.Length) ;
                        ParseMetaInfo(metaInfo);
                    }
                    _receivedBytes = 0;
                }
                var bytesLeft = _metaInt - _receivedBytes > count ?
                                  count : _metaInt - _receivedBytes;
                var result = _netStream.Read(buffer, offset, bytesLeft);
                _receivedBytes += result;
                return result;
            }
            catch (Exception e)
            {
                _connected = false;
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        
        public override void Close()
        {
            _connected = false;
            _netStream.Close();
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
        
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
