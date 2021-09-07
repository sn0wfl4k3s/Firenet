using Google.Cloud.Firestore;
using System;

namespace Firenet
{
    internal class DefaultNullableDatetimeConverter : IFirestoreConverter<DateTime?>
    {
        public object ToFirestore(DateTime? value)
            => value.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null;

        public DateTime? FromFirestore(object value)
            => value is null ? null : ((Timestamp)value).ToDateTime();
    }
}
