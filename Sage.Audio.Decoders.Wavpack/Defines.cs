/*
** Defines.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

// ReSharper disable InconsistentNaming

namespace Sage.Audio.Decoders.Wavpack
{
    public static class Defines
    {
        // Change the following value to an even number to reflect the maximum number of samples to be processed
        // per call to WavPackUtils.WavpackUnpackSamples

        internal const int SAMPLE_BUFFER_SIZE = 25600; // originally 5120
        
        // or-values for "flags"

        internal const int BYTES_STORED = 3; // 1-4 bytes/sample
        internal const int MONO_FLAG = 4; // not stereo
        internal const int HYBRID_FLAG = 8; // hybrid mode
        internal const int FALSE_STEREO = 0x40000000; // block is stereo, but data is mono

        internal const int SHIFT_LSB = 13;
        internal const long SHIFT_MASK = 0x1fL << SHIFT_LSB;

        internal const int FLOAT_DATA = 0x80; // ieee 32-bit floating point data

        internal const int SRATE_LSB = 23;
        internal const long SRATE_MASK = 0xfL << SRATE_LSB;

        internal const int FINAL_BLOCK = 0x1000; // final block of multichannel segment

        internal const int MIN_STREAM_VERS = 0x402; // lowest stream version we'll decode
        internal const int MAX_STREAM_VERS = 0x410; // highest stream version we'll decode

        internal const short ID_DUMMY = 0x0;
        internal const short ID_ENCODER_INFO = 0x1;
        internal const short ID_DECORR_TERMS = 0x2;
        internal const short ID_DECORR_WEIGHTS = 0x3;
        internal const short ID_DECORR_SAMPLES = 0x4;
        internal const short ID_ENTROPY_VARS = 0x5;
        internal const short ID_HYBRID_PROFILE = 0x6;
        internal const short ID_SHAPING_WEIGHTS = 0x7;
        internal const short ID_FLOAT_INFO = 0x8;
        internal const short ID_INT32_INFO = 0x9;
        internal const short ID_WV_BITSTREAM = 0xa;
        internal const short ID_WVC_BITSTREAM = 0xb;
        internal const short ID_WVX_BITSTREAM = 0xc;
        internal const short ID_CHANNEL_INFO = 0xd;

        internal const int JOINT_STEREO = 0x10; // joint stereo
        internal const int CROSS_DECORR = 0x20; // no-delay cross decorrelation
        internal const int HYBRID_SHAPE = 0x40; // noise shape (hybrid mode only)

        internal const int INT32_DATA = 0x100; // special extended int handling
        internal const int HYBRID_BITRATE = 0x200; // bitrate noise (hybrid mode only)
        internal const int HYBRID_BALANCE = 0x400; // balance noise (hybrid stereo mode only)

        internal const int INITIAL_BLOCK = 0x800; // initial block of multichannel segment

        internal const int FLOAT_SHIFT_ONES = 1; // bits left-shifted into float = '1'
        internal const int FLOAT_SHIFT_SAME = 2; // bits left-shifted into float are the same
        internal const int FLOAT_SHIFT_SENT = 4; // bits shifted into float are sent literally
        internal const int FLOAT_ZEROS_SENT = 8; // "zeros" are not all real zeros
        internal const int FLOAT_NEG_ZEROS = 0x10; // contains negative zeros
        internal const int FLOAT_EXCEPTIONS = 0x20; // contains exceptions (inf, nan, etc.)

        internal const short ID_OPTIONAL_DATA = 0x20;
        internal const int ID_ODD_SIZE = 0x40;
        internal const int ID_LARGE = 0x80;

        internal const int MAX_NTERMS = 16;
        internal const int MAX_TERM = 8;

        internal const int MAG_LSB = 18;
        internal const long MAG_MASK = 0x1fL << MAG_LSB;

        internal const short ID_RIFF_HEADER = 0x21;
        internal const short ID_RIFF_TRAILER = 0x22;
        internal const short ID_REPLAY_GAIN = 0x23;
        internal const short ID_CUESHEET = 0x24;
        internal const short ID_CONFIG_BLOCK = 0x25;
        internal const short ID_MD5_CHECKSUM = 0x26;
        internal const short ID_SAMPLE_RATE = 0x27;

        internal const long CONFIG_BYTES_STORED = 3; // 1-4 bytes/sample
        internal const long CONFIG_MONO_FLAG = 4; // not stereo
        internal const long CONFIG_HYBRID_FLAG = 8; // hybrid mode
        internal const long CONFIG_JOINT_STEREO = 0x10; // joint stereo
        internal const long CONFIG_CROSS_DECORR = 0x20; // no-delay cross decorrelation
        internal const long CONFIG_HYBRID_SHAPE = 0x40; // noise shape (hybrid mode only)
        internal const long CONFIG_FLOAT_DATA = 0x80; // ieee 32-bit floating point data
        internal const long CONFIG_FAST_FLAG = 0x200; // fast mode
        internal const long CONFIG_HIGH_FLAG = 0x800; // high quality mode
        internal const long CONFIG_VERY_HIGH_FLAG = 0x1000; // very high
        internal const long CONFIG_BITRATE_KBPS = 0x2000; // bitrate is kbps, not bits / sample
        internal const long CONFIG_AUTO_SHAPING = 0x4000; // automatic noise shaping
        internal const long CONFIG_SHAPE_OVERRIDE = 0x8000; // shaping mode specified
        internal const long CONFIG_JOINT_OVERRIDE = 0x10000; // joint-stereo mode specified
        internal const long CONFIG_CREATE_EXE = 0x40000; // create executable
        internal const long CONFIG_CREATE_WVC = 0x80000; // create correction file
        internal const long CONFIG_OPTIMIZE_WVC = 0x100000; // maximize bybrid compression
        internal const long CONFIG_CALC_NOISE = 0x800000; // calc noise in hybrid mode
        internal const long CONFIG_LOSSY_MODE = 0x1000000; // obsolete (for information)
        internal const long CONFIG_EXTRA_MODE = 0x2000000; // extra processing mode
        internal const long CONFIG_SKIP_WVX = 0x4000000; // no wvx stream w/ floats & big ints
        internal const long CONFIG_MD5_CHECKSUM = 0x8000000; // compute & store MD5 signature
        internal const long CONFIG_OPTIMIZE_MONO = 0x80000000; // optimize for mono streams posing as stereo

        internal const int MODE_WVC = 0x1;
        internal const int MODE_LOSSLESS = 0x2;
        internal const int MODE_HYBRID = 0x4;
        internal const int MODE_FLOAT = 0x8;
        internal const int MODE_VALID_TAG = 0x10;
        internal const int MODE_HIGH = 0x20;
        internal const int MODE_FAST = 0x40;
    }
}
