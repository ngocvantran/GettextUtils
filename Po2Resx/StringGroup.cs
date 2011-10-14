using System;

namespace Po2Resx
{
    public class StringGroup
    {
        private readonly string _language;
        private readonly string _path;

        public string Language
        {
            get { return _language; }
        }

        public string Path
        {
            get { return _path; }
        }

        public StringGroup(StringInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            _path = info.Path;
            _language = info.Language;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == typeof(StringGroup) &&
                Equals((StringGroup)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_path != null ? _path.GetHashCode() : 0)*397) ^
                    (_language != null ? _language.GetHashCode() : 0);
            }
        }

        private bool Equals(StringGroup other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(other._path, _path) &&
                Equals(other._language, _language);
        }
    }
}