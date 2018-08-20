﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Juno
{
    public class DIContainer
    {
        #region public
        public int AnonymousID
        {
            get;
            set;
        }

        public DIContainer( int anonymousID = int.MinValue )
        {
            m_namedBindings = new Dictionary<Type, Dictionary<int, object>>();
            m_anonymousBindings = new Dictionary<Type, List<object>>();
            m_injectQueue = new HashSet<object>();
            AnonymousID = anonymousID;
        }

        #region binding
        #region anonymous
        public void Bind<T>()
        {
            Type type = typeof( T );
            object instance = Activator.CreateInstance( type );
            Bind( type, instance );
        }

        public void Bind<T>( T instance )
        {
            Bind( typeof( T ), instance );
        }

        public void Bind( Type type, object instance )
        {
            var bindings = GetAnonymousBindings( type );
            bindings.Add( instance );
            QueueForInject( instance );
        }

        public void Unbind<T>()
        {
            var bindings = GetAnonymousBindings( typeof( T ) );
            bindings.Clear();
        }

        public bool TryGet<T>( out T instance )
        {
            var bindings = GetAnonymousBindings( typeof( T ) );
            if ( bindings.Count > 0 )
            {
                instance = ( T )bindings[0];
                return true;
            }
            else
            {
                instance = default( T );
                return false;
            }
        }

        public T Get<T>()
        {
            T instance;
            if ( TryGet( out instance ) )
            {
                return instance;
            }

            throw new KeyNotFoundException( string.Format( "DIContainer.Get: No anonymous bindings exist for type '{0}'", typeof( T ).FullName ) );
        }
        #endregion // anonymous

        #region named
        public void Bind<T>( int id )
        {
            Type type = typeof( T );
            object instance = Activator.CreateInstance( type );
            Bind( type, instance, id );
        }

        public void Bind<T>( T instance, int id )
        {
            Bind( typeof( T ), instance, id );
        }

        public void Bind( Type type, object instance, int id )
        {
            var typeBindings = GetNamedTypeBindings( type );
            typeBindings.Add( id, instance );
            QueueForInject( instance );
        }

        public void Unbind<T>( int id )
        {
            var bindings = GetNamedTypeBindings( typeof( T ) );
            bindings.Remove( id );
        }

        public bool TryGet<T>( int id, out T instance )
        {
            var bindings = GetNamedTypeBindings( typeof( T ) );
            object objInst;
            if ( bindings.TryGetValue( id, out objInst ) )
            {
                instance = ( T )objInst;
                return true;
            }
            else
            {
                instance = default( T );
                return false;
            }
        }

        public T Get<T>( int id )
        {
            T instance;
            if ( TryGet( id, out instance ) )
            {
                return instance;
            }

            throw new KeyNotFoundException( string.Format( "DIContainer.Get: No bindings exist for type '{0}' with id '{1}'", typeof( T ).FullName, id ) );
        }
        #endregion // named
        #endregion // binding

        #region injection
        public void Inject( object obj )
        {

        }

        public void FlushInjectQueue()
        {
            foreach ( object obj in m_injectQueue )
            {
                Inject( obj );
            }
            m_injectQueue.Clear();
        }

        public void QueueForInject( object obj )
        {
            m_injectQueue.Add( obj );
        }
        #endregion // injection
        #endregion // public

        #region private
        private Dictionary<Type, Dictionary<int, object>> m_namedBindings;
        private Dictionary<Type, List<object>> m_anonymousBindings;
        private HashSet<object> m_injectQueue;

        private List<object> GetAnonymousBindings( Type type )
        {
            List<object> bindings;

            if ( m_anonymousBindings.TryGetValue( type, out bindings ) == false )
            {
                bindings = new List<object>();
                m_anonymousBindings.Add( type, bindings );
            }

            return bindings;
        }

        private Dictionary<int, object> GetNamedTypeBindings( Type type )
        {
            Dictionary<int, object> typeBindings;
            if ( m_namedBindings.TryGetValue( type, out typeBindings ) == false )
            {
                typeBindings = new Dictionary<int, object>();
                m_namedBindings.Add( type, typeBindings );
            }

            return typeBindings;
        }
        #endregion // private
    }
}