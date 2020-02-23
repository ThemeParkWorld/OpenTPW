// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

namespace OpenTPW.RSSEQ
{
    // Some opcodes are missing entirely, and I'm pretty sure
    // ADDOBJ_3 isn't correct.
    // TODO: investigate further

    public enum OpcodeID
    {
        NOP = 0,            // No operation
        CRIT_LOCK = 1,      // Locks a ride; no more visitors can get on.
        CRIT_UNLOCK = 2,    // Unlocks a ride, allowing a previously-locked ride to be accessed freely by visitors once again.
        COPY = 3,           // Copies data into a specific location: COPY <to> <from / value>
        SUB = 5,            // Subtract a value from another: SUB <src> <value> <dest>
        ENDSLICE = 6,       // Unknown - likely has something to do with timeslice value
        GETTIME = 7,        // Gets the time that the ride has been alive/active for and puts it into a specific location: GETTIME <dest>
        ADDOBJ = 8,         // Add a object of a specific type to the ride: ADDOBJ <type> <parameter> <id> <slot>
        KILLOBJ = 10,       // Remove a object of a specific slot type: KILLOBJ <slot>
        FADEOBJ = 11,       // Unknown - likely KILLOBJ, but fading instead of immediately removing (used for visible objects?)
        SETOBJPARAM = 12,   // Set one of a particular object's parameters: SETOBJPARAM <slot> <parameter> <value>
        EVENT = 13,         // Trigger a global in-game event: EVENT <type> <unknown - parameter? length? - "node"> <event>
        FLUSHANIM = 15,     // Stop all active animations
        TRIGANIM = 16,      // Unknown
        WAITANIM = 17,      // Start playing an animation, and then wait for it to end: WAITANIM <type> <unknown>
        LOOPANIM = 18,      // Start playing an animation, and loop it: LOOPANIM <type> <unknown>
        TRIGWAITANIM = 19,  // Unknown
        TRIGANIMSPEED = 20, // Unknown
        TURBO = 21,         // Unknown
        TRIGANIM_CH = 23,   // Unknown - possibly trigger animation on visitor / CHaracter
        LOOPANIM_CH = 25,   // Unknown - possibly loop animation on visitor / CHaracter
        GETANIM_CH = 27,    // Unknown - possibly get animation on visitor / CHaracter
        RAND = 28,          // Choose a random number between 0 and max value, and put it into a particular variable: RAND <dest> <max value>
        JSR = 29,           // Jump to subroutine
        RETURN = 30,        // Return from subroutine
        BRANCH = 31,        // Branch to location (no returning)
        BRANCH_Z = 32,      // Branch if value is zero: BRANCH <value / variable>
        BRANCH_NZ = 33,     // Branch if value is NOT zero: BRANCH <value / variable>
        BRANCH_NV = 34,     // Branch if value is negative value: BRANCH <value / variable>
        BRANCH_PV = 35,     // Branch if value is positive value: BRANCH <value / variable>
        NAME = 37,          // Set ride's debugging name
        TEST = 38,          // Push a value onto the stack for use with branch commands?: TEST <value / variable>
        CMP = 39,           // Compare two values / variables for use with branch commands?: CMP <value / variable> <value / variable>
        HUSH = 42,          // Unknown
        HOP = 43,           // Unknown
        WAIT = 44,          // Wait for X cycles/slices/ms (unknown): WAIT <value>
        WAIT4ANIM = 46,     // Wait for all of the currently playing animations to finish.
        ADD = 47,           // Add a value to another: ADD <src> <value> <dest>
        DIV = 49,           // Divide values: DIV <src> <value> <dest>
        MOD = 50,           // Perform modular math on values: MOD <src> <value> <dest>
        ADDOBJ__3 = 51,     // ???
        TOUR = 53,          // Unknown - only used on tour rides
        BUMP = 54,          // Unknown - only used on rides with per-visitor objects
        COAST = 55,         // Unknown - only used on coasters
        ADDHEAD = 56,       // Add a visitor's head to the ride: ADDHEAD <visitor ID?>
        DELHEAD = 57,       // Remove a visitor's head from the ride: DELHEAD <visitor ID?>
        LIMBO = 58,         // "Hide" a visitor temporarily: LIMBO <visitor ID?> <unknown>
        UNLIMBO = 59,       // Un-hide a visitor: UNLIMBO <visitor ID?>
        FORCEUNLIMBO = 60,  // Force un-hide a visitor: FORCEUNLIMBO <visitor ID?>
        INLIMBO = 61,       // Unknown - check if any visitors are in limbo? INLIMBO <dest>
        LIMBOSPACE = 62,    // Unknown
        SPAWNCHILD = 63,    // Create another child VM with a pre-defined script from the same directory (max. 1 child script per ride?): SPAWNCHILD <script file name>
        SPAWNSOUND = 64,    // Unknown, only used for "EventMap.rse" files - possibly same as SPAWNCHILD: SPAWNCHILD <script file name>
        REMOVECHILD = 65,   // Remove child script: REMOVECHILD
        SETVARINCHILD = 66, // Set a child script's variable: SETVARINCHILD <variable ID> <value>
        GETVARINCHILD = 67, // Get a child script's variable: SETVARINCHILD <dest> <variable ID>
        SETVARINPARENT = 68,// SPECULATION - Set a parent script's variable: SETVARINCHILD <variable ID> <value>
        GETVARINPARENT = 69,// Get a parent script's variable: SETVARINCHILD <dest> <variable ID>
        BOUNCESETNODE = 70, // Unknown - set bouncing node?
        BOUNCESETBASE = 71, // Unknown - set bouncing base?
        BOUNCE = 72,        // Make a visitor bounce on a ride: BOUNCE <visitor ID> <duration>
        UNBOUNCE = 73,      // Stop a visitor bouncing on a ride: UNBOUNCE <visitor ID>
        FORCEUNBOUNCE = 74, // Stop a visitor bouncing on a ride (ignore bounce duration): FORCEUNBOUNCE <visitor ID>
        BOUNCING = 75,      // Check whether a visitor is bouncing on the ride: BOUNCING <visitor ID>
        WALKON = 76,        // Unknown
        WALKOFF = 77,       // Unknown
        WALKGET = 78,       // Unknown
        WALKST_FLOAT = 79,  // Unknown
        WALKFLOATSTAT = 80, // Unknown
        WALKFLOATSTOP = 81, // Unknown
        STARTSCREAM = 86,   // Unknown - cause visitors to scream?: STARTSCREAM <visitor ID?> <duration?>
        STOPSCREAM = 87,    // Stop all of a ride's screaming sfx
        SINGLESCREAM = 88,  // Unknown - cause a visitor to scream?: STARTSCREAM <visitor ID?> <duration?>
        SCREAMLEVEL = 89,   // Set the ride's "scream level"(?) to a particular value (out of 100): SCREAMLEVEL <value>
        FINDSCRIPTRAND = 90,// Find another ride / object at random, and put its script ID into a variable: FINDSCRIPTRAND <ride / obj name> <dest>
        SETREMOTEVAR = 92,  // Set another ride / object's script's variable: <script ID> <variable ID> <value>
        REPAIREFFECT = 93,  // Display the repair effect(?): REPAIREFFECT <1/0>
        SETTIMER = 95,      // Unknown
        GETTIMER = 96,      // Unknown
        /* SPECULATION */
        YEAR = 97,
        MONTH = 98,
        DAY = 99,
        /* end speculation */
        HOUR = 100,         // Get the current in-game hour: HOUR <dest>
        MIN = 101,          // Get the current in-game minute: MIN <dest>
        SEC = 102,          // Get the current in-game second: SEC <dest>
        SETREVERB = 103,    // Set sfx reverb levels from this ride - values ranging 0 to 10: SETREVERB <level>
        DIPMUSIC = 104,     // Mute/unmute music: DIPMUSIC <1/0>
        SPARK = 105         // Unknown
    }
}
