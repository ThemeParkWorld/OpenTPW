using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW.Formats.Sound;
public class MP2Stream : BaseStream
{
	public MP2File MP2File;
	public MP2Stream( byte[] buffer ) : base( buffer ) { }
}
