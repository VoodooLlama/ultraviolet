﻿using System;

namespace TwistedLogik.Ultraviolet.UI.Presentation
{
    /// <summary>
    /// Represents the set of options which can be used to configure the binding expressions compiler.
    /// </summary>
    public class BindingExpressionCompilerOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the compiler should perform a compilation regardless of whether
        /// there is an up-to-date cache file.
        /// </summary>
        public Boolean IgnoreCache
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the compiler should work in the user's temporary directory
        /// rather than the application directory.
        /// </summary>
        public Boolean WorkInTemporaryDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value specifying whether to write errors to a file in the working directory.
        /// </summary>
        public Boolean WriteErrorsToFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the compiler will write its generated files to the working directory
        /// even if no errors occur.
        /// </summary>
        public Boolean WriteCompiledFilesToWorkingDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the compiler input.
        /// </summary>
        public String Input
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the compiler output.
        /// </summary>
        public String Output
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the requested namespace of the compiled view models.
        /// </summary>
        public String RequestedViewModelNamespace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the requested name of the compiled view model.
        /// </summary>
        public String RequestedViewModelName
        {
            get;
            set;
        }
    }
}
