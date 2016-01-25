﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using TwistedLogik.Ultraviolet.UI.Presentation.Uvss.Syntax;
using TwistedLogik.Ultraviolet.VisualStudio.Uvss.Errors;
using TwistedLogik.Ultraviolet.VisualStudio.Uvss.Parsing;

namespace TwistedLogik.Ultraviolet.VisualStudio.Uvss.Tagging
{
	/// <summary>
	/// Represents a tagger which puts squigglies under parser errors.
	/// </summary>
	internal sealed class SyntaxErrorTagger : ITagger<IErrorTag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxErrorTagger"/> class.
        /// </summary>
        /// <param name="parserService">The UVSS language parser service.</param>
        /// <param name="buffer">The text buffer that contains the text being tagged.</param>
        public SyntaxErrorTagger(IUvssParserService parserService, ITextBuffer buffer)
        {
            this.parserService = parserService;
            this.parserService.DocumentGenerated += (span, document) =>
            {
                if (span.Snapshot.TextBuffer == buffer)
                {
					RaiseTagsChanged(span);
                }
            };
            this.buffer = UvssTextBuffer.ForBuffer(buffer);
			this.buffer.CommentSpanInvalidated += (obj, span) =>
				RaiseTagsChanged(new SnapshotSpan(this.buffer.Buffer.CurrentSnapshot, span));
		}
        
        /// <summary>
        /// Gets the tags for the specified spans of text.
        /// </summary>
        /// <param name="spans">The spans of text for which to retrieve tags.</param>
        /// <returns>A collection containing the tags for the specified spans of text.</returns>
        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var result = new List<ITagSpan<IErrorTag>>();

            var errorList = this.buffer.Buffer.GetErrorList();
            if (errorList != null)
                errorList.Update();

			foreach (var span in spans)
			{
				var tagSpan = buffer.GetOutermostBlockSpan(span);
				var tags = GetTags(tagSpan);
				result.AddRange(tags);
			}

            return result;
        }

        /// <summary>
        /// Gets the tags for the specified document.
        /// </summary>
        /// <param name="span">The snapshot span from which the document was generated.</param>
        /// <returns>A collection containing the tags for the specified document.</returns>
        public IEnumerable<ITagSpan<IErrorTag>> GetTags(SnapshotSpan span)
        {
            var result = new List<ITagSpan<IErrorTag>>();

            var errorList = span.Snapshot.TextBuffer.GetErrorList();
            if (errorList == null)
                return Enumerable.Empty<ITagSpan<IErrorTag>>();

            var errors = errorList.GetErrorsInSpan(span);

            return errors.Select(x =>
            {
                var tagSpan = x.TagSafeSpan;
                var tag = new ErrorTag(x.DiagnosticInfo.Message, x.DiagnosticInfo.Message);
                return new TagSpan<IErrorTag>(tagSpan, tag);
            });
        }

        /// <inheritdoc/>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Raises the <see cref="TagsChanged"/> event.
        /// </summary>
        private void RaiseTagsChanged(SnapshotSpan span)
        {
            var handler = TagsChanged;
            if (handler != null)
            {
                handler(this, new SnapshotSpanEventArgs(span));
            }
        }
        
        // Tagging services.
        private readonly IUvssParserService parserService;
        private readonly UvssTextBuffer buffer; 
    }
}
