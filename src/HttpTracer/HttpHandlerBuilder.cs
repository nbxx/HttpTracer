﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace HttpTracer
{
    public class HttpHandlerBuilder
    {
        private readonly IList<HttpMessageHandler> _handlersList = new List<HttpMessageHandler>();
        private readonly HttpTracerHandler _rootHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HttpTracer.HttpHandlerBuilder"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public HttpHandlerBuilder(ILogger logger = null)
        {
            _rootHandler = new HttpTracerHandler(null, logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HttpTracer.HttpHandlerBuilder"/> class.
        /// </summary>
        /// <param name="tracerHandler">Tracer handler.</param>
        public HttpHandlerBuilder(HttpTracerHandler tracerHandler)
        {
            _rootHandler = tracerHandler;
        }

        /// <summary>
        /// Adds a <see cref="HttpMessageHandler"/> to the chain of handlers.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public HttpHandlerBuilder AddHandler(HttpMessageHandler handler)
        {
            if (handler is HttpTracerHandler) throw new ArgumentException($"Can't add handler of type {nameof(HttpTracerHandler)}.");

            if (_handlersList.Any())
                ((DelegatingHandler)_handlersList.LastOrDefault()).InnerHandler = handler;

            _handlersList.Add(handler);
            return this;
        }

        /// <summary>
        /// Adds <see cref="HttpTracerHandler"/> as the last link of the chain.
        /// </summary>
        /// <returns></returns>
        public HttpMessageHandler Build()
        {
            if (_handlersList.Any())
                ((DelegatingHandler)_handlersList.LastOrDefault()).InnerHandler = _rootHandler;
            else
                return _rootHandler;

            return _handlersList.FirstOrDefault();
        }
    }
}
