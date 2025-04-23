namespace Infuse
{
    /** 
     * InfuseContext is the common interface to an Infuse Context.
     */
    public interface InfuseContext
    {
        /**
         * Register an object instance with this Infuse Context.
         * @param instance The object instance to register.
         * @param unregisterOnDestroy If true, the object will be unregistered when it is destroyed.
         */
        void Register(object instance, bool unregisterOnDestroy = true);

        /**
         * Unregister an object instance from this Infuse Context.
         * @param instance The object instance to unregister.
         */
        void Unregister(object instance);

        /**
         * Register a service instance with this Infuse Context.
         * @param instance The service instance to register.
         * @typeparam TServiceType The type of the service.
         */
        void RegisterService<TServiceType>(object instance) where TServiceType : class;

        /**
         * Unregister a service instance from this Infuse Context.
         * @param instance The service instance to unregister.
         * @typeparam TServiceType The type of the service.
         */
        void UnregisterService<TServiceType>(object instance) where TServiceType : class;
    }
}
