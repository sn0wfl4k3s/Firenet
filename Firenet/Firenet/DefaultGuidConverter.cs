using Google.Cloud.Firestore;
using System;

namespace Firenet
{
    public class DefaultGuidConverter : IFirestoreConverter<Guid>
    {
        public Guid FromFirestore(object value)
            => Guid.Parse(value.ToString());

        public object ToFirestore(Guid value)
            => value.ToString();
    }
}
