namespace Uno.Server.Annotation
{
    /// <summary>
    /// Used to mark classe as services, to be automatically added to the DI.
    /// </summary>
    public class ServiceAttribute : Attribute
    {
        public bool AsSingleton { get; set; }
    }
}
