using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.Exceptions
{
    public class SlugIsDuplicateDomainException : BaseDomainException
    {
        public SlugIsDuplicateDomainException() : base("slug تکراری است")
        {
        }

        public SlugIsDuplicateDomainException(string message) : base(message)
        {
        }
    }
}
