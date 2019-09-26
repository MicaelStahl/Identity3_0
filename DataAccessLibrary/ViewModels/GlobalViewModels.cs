using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Identity3_0.ViewModels
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
        NotFound,

        /// <summary>
        /// Indicating the list of objects is empty.
        /// </summary>
        [EnumMember(Value = "List is empty")]
        Empty,

        /// <summary>
        /// Indicates the action failed.
        /// </summary>
        [EnumMember(Value = "Action failed.")]
        Failed,

        /// <summary>
        /// Indicates not all required fields were filled on create or edit.
        /// </summary>
        [EnumMember(Value = "Please fill all fields.")]
        FillAllFields,

        /// <summary>
        /// Indicates the requested age is invalid.
        /// </summary>
        [EnumMember(Value = "Invalid age. Age has to be between 1 to 110.")]
        InvalidAge,

        /// <summary>
        /// Indicates the action was successful.
        /// </summary>
        [EnumMember(Value = "Action was successful")]
        Success,

        /// <summary>
        /// Indicates the object was successfully created.
        /// </summary>
        [EnumMember(Value = "Object was successfully created.")]
        Created,

        /// <summary>
        /// Indicates the object was successfully updated.
        /// </summary>
        [EnumMember(Value = "Object was successfully updated.")]
        Updated,

        /// <summary>
        /// Indicates the object was successfully deleted.
        /// </summary>
        [EnumMember(Value = "Object was successfully deleted.")]
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

    #endregion
}
