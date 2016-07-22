namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides a name for a <see cref="Node"/>
    /// </summary>
    internal interface INamedNode
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        string Name
        {
            get;
        }
    }
}