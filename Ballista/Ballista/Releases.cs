using System.Collections.Generic;

namespace Ballista
{
    class Releases
    {
        public static Dictionary<int, Dictionary<string, byte>> syscallOffsets = new Dictionary<int, Dictionary<string, byte>>()
        {
            //Parts of the dictionary taken from badBounty/directInjectorPOC

            /*              Windows 10                  */
             { 1, new Dictionary<string, byte>()       //W10-20H2, WS-2019
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xC1},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 2, new Dictionary<string, byte>()        //W10-2004, WS-2019
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xC1},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 3, new Dictionary<string, byte>()        //W10-1909, WS-2019
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xBD},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 4, new Dictionary<string, byte>()        //W10-1903, WS-2019
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xBD},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 5, new Dictionary<string, byte>()        //W10-1809, WS-2019
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xBC},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 6, new Dictionary<string, byte>()        //W10-1803
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xBB},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 7, new Dictionary<string, byte>()        //W10-1709
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xBA},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 8, new Dictionary<string, byte>()        //W10-1703
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xB9},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 9, new Dictionary<string, byte>()        //W10-1607, WS-2016
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xB6},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 10, new Dictionary<string, byte>()       //W10-1511, WS-2016
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xB4},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },
            { 11, new Dictionary<string, byte>()      //W10-1507, WS-2016
                {
                    { "NtOpenProcess",      0x26},
                    { "NtCreateThreadEx",   0xB3},
                    { "NtCreateSection",    0x4A},
                    { "NtMapViewOfSection", 0x28}
                }
            },

            /*              Windows 7                   */
            { 12, new Dictionary<string, byte>()      //W7-SP1 + SP0
                {
                    { "NtOpenProcess",      0x23},
                    { "NtCreateThreadEx",   0xA5},
                    { "NtCreateSection",    0x47},
                    { "NtMapViewOfSection", 0x25}
                }
            },

            /*       Windows Server 2012                */
            { 13, new Dictionary<string, byte>()      //WS2012-R2
                {
                    { "NtOpenProcess",      0x25},
                    { "NtCreateThreadEx",   0xB0},
                    { "NtCreateSection",    0x49},
                    { "NtMapViewOfSection", 0x27}
                }
            },
            { 14, new Dictionary<string, byte>()      //WS2012-SP0
                {
                    { "NtOpenProcess",      0x24},
                    { "NtCreateThreadEx",   0xAF},
                    { "NtCreateSection",    0x48},
                    { "NtMapViewOfSection", 0x26}
                }
            },
        };
    }
}
