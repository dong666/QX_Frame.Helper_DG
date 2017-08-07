using System;
/**
* author:qixiao
* create:2017-8-7 10:19:35
* */
namespace QX_Frame.Helper_DG.Bantina
{
    /// <summary>
    /// Self Define Attribute , Support Properties/Class Support Extend Inherit
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Real TableName : if not declare , use class name.
        /// </summary>
        public string TableName { get; set; }
    }
}
