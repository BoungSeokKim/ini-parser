using System;
using System.Text.RegularExpressions;

namespace IniParser.Model
{

    /// <summary>
    /// This structure defines the format of the INI file by customization the characters used to define sections
    /// key/values or comments.
    /// Used IniDataParser to read INI files, and an IIniDataFormatter to write a new ini file string.
    /// </summary>
	public class IniScheme : IDeepCloneable<IniScheme>
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <remarks>
        ///     By default the various delimiters for the data are setted:
        ///     <para>';' for one-line comments</para>
        ///     <para>'[' ']' for delimiting a section</para>
        ///     <para>'=' for linking key / value pairs</para>
        ///     <example>
        ///         An example of well formed data with the default values:
        ///         <para>
        ///         ;section comment<br/>
        ///         [section] ; section comment<br/>
        ///         <br/>
        ///         ; key comment<br/>
        ///         key = value ;key comment<br/>
        ///         <br/>
        ///         ;key2 comment<br/>
        ///         key2 = value<br/>
        ///         </para>
        ///     </example>
        /// </remarks>
        public IniScheme() {

            CommentString = ";";
			PropertyDelimiterString = "=";
            _sectionStartString = "[";
            _sectionEndString = "]";
			RecreateSectionRegex();
        }

        /// <summary>
        ///     Copy ctor.
        /// </summary>
        /// <param name="ori">
        ///     Original instance to be copied.
        /// </param>
        public IniScheme(IniScheme ori)
        {
            CommentString = ori.CommentString;
            PropertyDelimiterString = ori.PropertyDelimiterString;
            _sectionStartString = ori.SectionStartString;
            _sectionEndString = ori.SectionEndString;
            RecreateSectionRegex();
        }


        public Regex CommentRegex { get; set; }

        public Regex SectionRegex { get; set; }

        /// <summary>
        ///     Sets the char that defines the start of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character '['
        /// </remarks>
        public string SectionStartString
        {
			get { return _sectionStartString; }
            set
            {
				_sectionStartString = value;
				ThrowExceptionOnInvalidSectionRegex(_sectionStartString);
				RecreateSectionRegex();
            }
        }

        /// <summary>
        ///     Sets the char that defines the end of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character ']'
        /// </remarks>
        public string SectionEndString
        {
			get { return _sectionEndString; }
            set
            {
				_sectionEndString = value;
				ThrowExceptionOnInvalidSectionRegex(_sectionEndString);
				RecreateSectionRegex();
            }
        }

        /// <summary>
        ///     Sets the string that defines the start of a comment.
        ///     A comment spans from the mirst matching comment string
        ///     to the end of the line.
        /// </summary>
        /// <remarks>
        ///     Defaults to string ";"
        /// </remarks>
        public string CommentString
        {

            get { return _commentString ?? string.Empty; }
            set
            {
                // Sanitarize special characters for a regex
                foreach (var specialChar in _strSpecialRegexChars)
                {
                    value = value.Replace(new String(specialChar, 1), @"\" + specialChar);
                }

                CommentRegex = new Regex(string.Format(_strCommentRegex, value));
                _commentString = value;
            }
        }

        /// <summary>
        ///     Sets the string used in the ini file to denote a key / value assigment
        /// </summary>
        /// <remarks>
        ///     Defaults to character '='
        /// </remarks>
        public string PropertyDelimiterString { get; set; }

        #region Fields
        string _sectionStartString = string.Empty;
		string _sectionEndString = string.Empty;
		string _commentString = string.Empty;
        #endregion

        #region Constants
        protected const string _strCommentRegex = @"^{0}(.*)";
        protected const string _strSectionRegexStart = @"^(\s*?)";
        protected const string _strSectionRegexMiddle = @"{1}\s*[\p{L}\p{P}\p{M}_\""\'\{\}\#\+\;\*\%\(\)\=\?\&\$\,\:\/\.\-\w\d\s\\\~]+\s*";
        protected const string _strSectionRegexEnd = @"(\s*?)$";
        protected const string _strKeyRegex = @"^(\s*[_\.\d\w]*\s*)";
        protected const string _strValueRegex = @"([\s\d\w\W\.]*)$";
        protected const string _strSpecialRegexChars = @"[]\^$.|?*+()";
		#endregion

		#region Helpers

		private void ThrowExceptionOnInvalidSectionRegex(string value)
		{
			if (CommentString.Contains(value))
			{

				throw new Exception(string.Format("Section delimiter string '{0}' is equal or similar to comment string: '{1}'",
				                                  value,
				                                  CommentString));
			}

			if (PropertyDelimiterString.Contains(value)) {
				throw new Exception(string.Format("Section delimiter string '{0}' is equal or similar to property assignment string '{1}'",
				                                  value,
				                                  PropertyDelimiterString));
			}

		}

        private void RecreateSectionRegex()
        {
            string builtRegexString = _strSectionRegexStart;

            if (_strSpecialRegexChars.Contains(_sectionStartString))
				builtRegexString += "\\" + _sectionStartString;
			else builtRegexString += _sectionStartString;

            builtRegexString += _strSectionRegexMiddle;

			if (_strSpecialRegexChars.Contains(_sectionEndString))
				builtRegexString += "\\" + _sectionEndString;
            else
				builtRegexString += _sectionEndString;

            builtRegexString += _strSectionRegexEnd;

            SectionRegex = new Regex(builtRegexString);
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public IniScheme DeepClone()
        {
            return new IniScheme(this);
        }

        #endregion
    }

}
