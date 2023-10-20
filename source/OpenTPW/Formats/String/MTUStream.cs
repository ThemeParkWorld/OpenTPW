using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW.Formats.String;
internal sealed class MTUStream : BaseStream
{
	public MTUStream( byte[] buffer ) : base( buffer ) { }
}
