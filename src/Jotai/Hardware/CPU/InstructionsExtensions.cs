/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2014 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

namespace Jotai.Hardware.CPU
{
    public class InstructionsExtensions
    {
        public bool MMX { get; internal set; }

        public bool MMXExt { get; internal set; }

        public bool _3DNow { get; internal set; }

        public bool _3DNowExt { get; internal set; }

        public bool SSE { get; internal set; }

        public bool SSE2 { get; internal set; }

        public bool SSE3 { get; internal set; }

        public bool SSSE3 { get; internal set; }

        public bool SSE4a { get; internal set; }

        public bool SSE4_1 { get; internal set; }

        public bool SSE4_2 { get; internal set; }

        public bool AVX { get; internal set; }

        public bool AVX2 { get; internal set; }

        public bool AVX512F { get; internal set; }

        public bool TSX { get; internal set; }

        public bool FMA3 { get; internal set; }

        public bool FMA4 { get; internal set; }

        public bool AES { get; internal set; }
    }
}
