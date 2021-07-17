using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Firenet
{
    internal class DocumentSnapshotComparer : IEqualityComparer<DocumentSnapshot>
    {
        public bool Equals(DocumentSnapshot x, DocumentSnapshot y)
        {
            return x.GetHashCode().Equals(y.GetHashCode());
        }

        public int GetHashCode([DisallowNull] DocumentSnapshot obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
