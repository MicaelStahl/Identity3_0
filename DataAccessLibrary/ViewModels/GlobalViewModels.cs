using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DataAccessLibrary.ViewModels
{
    #region Global variables

    /// <summary>
    /// Enum containing various messages indicating success, fail etc.
    /// </summary>
    public enum ActionMessages
    {
        /// <summary>
        /// Indicates the requested object could not be found.
        /// </summary>
        [EnumMember(Value = "Not Found")]
        [Display(Name = "The requested object could not be found.")]
        NotFound,

        /// <summary>
        /// Indicating the list of objects is empty.
        /// </summary>
        [EnumMember(Value = "List is empty")]
        [Display(Name = "The requested list was empty inclining none exists or a database error.")]
        Empty,

        /// <summary>
        /// Indicates the action failed.
        /// </summary>
        [EnumMember(Value = "Action failed.")]
        [Display(Name = "The requested action failed.")]
        Failed,

        /// <summary>
        /// Indicates not all required fields were filled on create or edit.
        /// </summary>
        [EnumMember(Value = "Please fill all fields.")]
        [Display(Name = "Please fill all fields then try again.")]
        FillAllFields,

        /// <summary>
        /// Indicates the requested age is invalid.
        /// </summary>
        [EnumMember(Value = "Invalid age. Age has to be between 1 to 110.")]
        [Display(Name = "Age has to be between 1 to 110.")]
        InvalidAge,

        /// <summary>
        /// Indicates the action was successful.
        /// </summary>
        [EnumMember(Value = "Action was successful")]
        [Display(Name = "The requested action succeeded.")]
        Success,

        /// <summary>
        /// Indicates the object was successfully created.
        /// </summary>
        [EnumMember(Value = "Object was successfully created.")]
        [Display(Name = "The requested object was created.")]
        Created,

        /// <summary>
        /// Indicates the object was successfully updated.
        /// </summary>
        [EnumMember(Value = "Object was successfully updated.")]
        [Display(Name = "The requested object was updated.")]
        Updated,

        /// <summary>
        /// Indicates the object was successfully deleted.
        /// </summary>
        [EnumMember(Value = "Object was successfully deleted.")]
        [Display(Name = "The requested object was deleted.")]
        Deleted
    }

    /// <summary>
    /// A viewmodel containing an objects Id and a actionmessage.
    /// </summary>
    public class UpdatedObjectWithMessage
    {
        public Guid Id { get; set; }

        public ActionMessages Message { get; set; }
    }

    public class DictionaryMessages
    {
        public Dictionary<int, string> Messages { get; set; } = EnumToDictionary();

        /// <summary>
        /// Converts the ActionMessages enum to a dictionary.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, string> EnumToDictionary()
        {
            return Enum.GetValues(typeof(ActionMessages))
                    .Cast<ActionMessages>()
                    .ToDictionary(t => (int)t, t => Enum.GetName(typeof(ActionMessages), t));
        }
    }

    #endregion Global variables
}