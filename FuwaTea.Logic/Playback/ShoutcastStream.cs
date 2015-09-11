using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FuwaTea.Common.Models;

namespace FuwaTea.Logic.Playback
{
    public class ShoutcastStream : Stream
    {
        private readonly int _metaInt;
        private int _receivedBytes;
        private readonly Stream _netStream;
        private bool _connected;

        private int _read;
        private int _leftToRead;
        private int _thisOffset;
        private int _bytesRead;
        private int _bytesLeftToMeta;
        private int _metaLen;
        private byte[] _metaInfo;
        
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

            var newStreamTitle = Regex.Match(metaString, "(StreamTitle=')(.*)(';StreamUrl)").Groups[2].Value.Trim();
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

        public string StreamTitle { get { return StreamMetadata.Title; } private set { StreamMetadata.Title = value; } }
        public string MimeType { get; }

        public override void Flush() { }
        
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }
        
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                _read = 0;
                _leftToRead = count;
                _thisOffset = offset;
                _bytesRead = 0;
                _bytesLeftToMeta = ((_metaInt - _receivedBytes) > count) ? count : (_metaInt - _receivedBytes);

                while (_bytesLeftToMeta > 0 && (_read = _netStream.Read(buffer, _thisOffset, _bytesLeftToMeta)) > 0)
                {
                    _leftToRead -= _read;
                    _thisOffset += _read;
                    _bytesRead += _read;
                    _receivedBytes += _read;
                    _bytesLeftToMeta -= _read;
                }

                // read metadata
                if (_receivedBytes == _metaInt)
                {
                    ReadMetaData();
                }

                while (_leftToRead > 0 && (_read = _netStream.Read(buffer, _thisOffset, _leftToRead)) > 0)
                {
                    _leftToRead -= _read;
                    _thisOffset += _read;
                    _bytesRead += _read;
                    _receivedBytes += _read;
                }

                return _bytesRead;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private void ReadMetaData()
        {
            _metaLen = _netStream.ReadByte();
            if (_metaLen > 0)
            {
                _metaInfo = new byte[_metaLen * 16];
                var len = 0;
                while ((len += _netStream.Read(_metaInfo, len, _metaInfo.Length - len)) < _metaInfo.Length) ;
                ParseMetaInfo(_metaInfo);
            }
            _receivedBytes = 0;
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
