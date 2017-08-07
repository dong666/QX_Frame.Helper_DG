using System;

/**
 * author:qixiao
 * create:2017-8-7 10:36:22
 * */
namespace QX_Frame.Helper_DG.Bantina
{
    /// <summary>
    /// Self Define Attribute , Support Properties/Class Support Extend Inherit
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class AutoIncreaseAttribute : Attribute
    {
    }
}
