using System;
using UnityEngine;

#nullable enable

namespace api
{
    /// <summary>
    /// A promise is a value that will be assigned in the future.
    /// </summary>
    /// <typeparam name="T">The type of the value that will be returned.</typeparam>
    public class Promise<T>
    {
        public bool Finished { get; protected set; }

        protected T? value;
        protected Exception? error;

        /// <summary>
        /// Fulfils the promise and makes any calls to <see cref="Promise{T}.Get(out Exception?, float)"/> return the specified value
        /// </summary>
        /// <param name="value">The value to return</param>
        public void Fulfil(T value)
        {
            if (Finished)
                throw new System.InvalidOperationException("Cannot finish the same promise twice.");

            this.value = value;
            Finished = true;
        }

        /// <summary>
        /// Fails the promise and makes any calls to <see cref="Promise{T}.Get(out Exception?, float)"/> return the specified error
        /// </summary>
        /// <param name="error">The error to return</param>
        public void Fail(Exception error)
        {
            if (Finished)
                throw new System.InvalidOperationException("Cannot finish the same promise twice.");

            this.error = error;
            Finished = true;
        }

        /// <summary>
        /// Gets the returned value of a promise, blocks until a value is returned
        /// </summary>
        /// <param name="error">The object the error(if any) is assigned to upon the function returning null</param>
        /// <param name="timeout">The timeout in seconds to give up after and throw a <see cref="System.TimeoutException"/></param>
        /// <returns>The value of the promise, which could be null</returns>
        public Exception? Get(out T? value, float timeout = 10)
        {
            float endTime = Time.realtimeSinceStartup + timeout;

            while (Time.realtimeSinceStartup < endTime)
            {
                if (Finished)
                {
                    if (this.value != null)
                    {
                        value = this.value;
                        return null;
                    }
                    else if (this.error != null)
                    {
                        value = default;
                        return this.error;
                    }
                    else
                    {
                        throw new System.Exception();//TODO, make actual exception
                    }
                }
            }

            throw new System.TimeoutException("Get operation exceeded timeout");
        }
    }
}