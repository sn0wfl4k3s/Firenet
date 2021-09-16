using Google.Cloud.Firestore;
using System;

namespace Firenet
{
    internal class DefaultDatetimeConverter : IFirestoreConverter<DateTime>
    {
        public object ToFirestore(DateTime value)
            => Timestamp.FromDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc));

        public DateTime FromFirestore(object value)
            => ((Timestamp)value).ToDateTime();
    }
}
