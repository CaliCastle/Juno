﻿using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public abstract class ContextBase : MonoBehaviour
    {
        #region public
        public DIContainer Container
        {
            get;
            private set;
        }
        #endregion // public

        #region protected
        protected virtual void OnPreInject()
        {
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected void InstallBindings<T>( List<T> installers ) where T : IInstaller
        {
            if ( installers != null )
            {
                for ( int i = 0; i < installers.Count; i++ )
                {
                    IInstaller installer = installers[i];
                    if ( installer != null )
                    {
                        installer.InstallBindings( Container );
                    }
                    else
                    {
                        throw new System.NullReferenceException( string.Format( "Null installer reference found in context '{0}'", name ) );
                    }
                } 
            }
        }
        #endregion // protected

        #region private
        [SerializeField] private List<MonoBehaviourInstaller> m_monoBehaviourInstallers;
        [SerializeField] private List<ScriptableObjectInstaller> m_scriptableObjectInstallers;

        private void Awake()
        {
            Container = new DIContainer();

            // run installers
            InstallBindings( m_monoBehaviourInstallers );
            InstallBindings( m_scriptableObjectInstallers );

            OnAwake();
        }

        private void Start()
        {
            // inject
            OnPreInject();
            Container.FlushInjectQueue();

            OnStart();
        }
        #endregion // private
    }
}