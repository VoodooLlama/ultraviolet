﻿using System;
using System.Collections.Generic;
using System.Text;
using TwistedLogik.Nucleus;
using TwistedLogik.Nucleus.Text;

namespace TwistedLogik.Ultraviolet.Graphics.Graphics2D.Text
{
    /// <summary>
    /// Represents an engine for laying out formatted text.
    /// </summary>
    public sealed partial class TextLayoutEngine
    {
        /// <summary>
        /// Registers a style with the specified name.
        /// </summary>
        /// <param name="name">The name of the style to register.</param>
        /// <param name="style">The style to register.</param>
        public void RegisterStyle(String name, TextStyle style)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            registeredStyles.Add(name, style);
        }

        /// <summary>
        /// Registers the font with the specified name.
        /// </summary>
        /// <param name="name">The name of the font to register.</param>
        /// <param name="font">The font to register.</param>
        public void RegisterFont(String name, SpriteFont font)
        {
            Contract.RequireNotEmpty(name, nameof(name));
            Contract.Require(font, nameof(font));

            registeredFonts.Add(name, font);
        }

        /// <summary>
        /// Registers the icon with the specified name.
        /// </summary>
        /// <param name="name">The name of the icon to register.</param>
        /// <param name="icon">The icon to register.</param>
        /// <param name="height">The width to which to scale the icon, or null to preserve the sprite's original width.</param>
        /// <param name="width">The height to which to scale the icon, or null to preserve the sprite's original height.</param>
        public void RegisterIcon(String name, SpriteAnimation icon, Int32? width = null, Int32? height = null)
        {
            Contract.RequireNotEmpty(name, nameof(name));
            Contract.Require(icon, nameof(icon));

            registeredIcons.Add(name, new TextIconInfo(icon, width, height));
        }

        /// <summary>
        /// Registers the glyph shader with the specified name.
        /// </summary>
        /// <param name="name">The name of the glyph shader to register.</param>
        /// <param name="shader">The glyph shader to register.</param>
        public void RegisterGlyphShader(String name, GlyphShader shader)
        {
            Contract.RequireNotEmpty(name, nameof(name));
            Contract.Require(shader, nameof(shader));

            registeredGlyphShaders.Add(name, shader);
        }

        /// <summary>
        /// Unregisters the style with the specified name.
        /// </summary>
        /// <param name="name">The name of the style to unregister.</param>
        /// <returns><see langword="true"/> if the style was unregistered; otherwise, <see langword="false"/>.</returns>
        public Boolean UnregisterStyle(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            return registeredStyles.Remove(name);
        }

        /// <summary>
        /// Unregisters the font with the specified name.
        /// </summary>
        /// <param name="name">The name of the font to unregister.</param>
        /// <returns><see langword="true"/> if the font was unregistered; otherwise, <see langword="false"/>.</returns>
        public Boolean UnregisterFont(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            return registeredFonts.Remove(name);
        }

        /// <summary>
        /// Unregisters the icon with the specified name.
        /// </summary>
        /// <param name="name">The name of the icon to unregister.</param>
        /// <returns><see langword="true"/> if the icon was unregistered; otherwise, <see langword="false"/>.</returns>
        public Boolean UnregisterIcon(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            return registeredIcons.Remove(name);
        }

        /// <summary>
        /// Unregisters the glyph shader with the specified name.
        /// </summary>
        /// <param name="name">The name of the glyph shader to unregister.</param>
        /// <returns><see langword="true"/> if the glyph shader was unregistered; otherwise, <see langword="false"/>.</returns>
        public Boolean UnregisterGlyphShader(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            return registeredGlyphShaders.Remove(name);
        }

        /// <summary>
        /// Calculates a layout for the specified text.
        /// </summary>
        /// <param name="input">The parsed text which will be laid out according to the specified settings.</param>
        /// <param name="output">The layout command stream which will be populated with commands by this operation.</param>
        /// <param name="settings">A <see cref="TextLayoutSettings"/> structure which contains the settings for this operation.</param>
        public void CalculateLayout(TextParserTokenStream input, TextLayoutCommandStream output, TextLayoutSettings settings)
        {
            Contract.Require(input, nameof(input));
            Contract.Require(output, nameof(output));

            if (settings.Font == null)
                throw new ArgumentException(UltravioletStrings.InvalidLayoutSettings);

            var state = new LayoutState() { LineInfoCommandIndex = 1 };

            output.Clear();
            output.SourceText = input.SourceText;
            output.ParserOptions = input.ParserOptions;

            var acquiredPointers = !output.HasAcquiredPointers;
            if (acquiredPointers)
                output.AcquirePointers();

            output.WriteBlockInfo();
            output.WriteLineInfo();

            var bold = (settings.Style == SpriteFontStyle.Bold || settings.Style == SpriteFontStyle.BoldItalic);
            var italic = (settings.Style == SpriteFontStyle.Italic || settings.Style == SpriteFontStyle.BoldItalic);

            if (settings.InitialLayoutStyle != null)
                PrepareInitialStyle(output, ref bold, ref italic, ref settings);

            var currentFont = settings.Font;
            var currentFontFace = settings.Font.GetFace(SpriteFontStyle.Regular);

            var index = 0;
            var processing = true;

            while (index < input.Count && processing)
            {
                if (state.PositionY >= (settings.Height ?? Int32.MaxValue))
                    break;

                var token = input[index];

                currentFontFace = default(SpriteFontFace);
                currentFont = GetCurrentFont(ref settings, bold, italic, out currentFontFace);

                switch (token.TokenType)
                {
                    case TextParserTokenType.Text:
                        processing = ProcessTextToken(input, output, currentFontFace, ref token, ref state, ref settings, ref index);
                        break;

                    case TextParserTokenType.Icon:
                        processing = ProcessIconToken(output, ref token, ref state, ref settings, ref index);
                        break;

                    case TextParserTokenType.ToggleBold:
                        ProcessToggleBoldToken(output, ref bold, ref state, ref index);
                        break;

                    case TextParserTokenType.ToggleItalic:
                        ProcessToggleItalicToken(output, ref italic, ref state, ref index);
                        break;

                    case TextParserTokenType.PushFont:
                        ProcessPushFontToken(output, ref token, ref state, ref index);
                        break;

                    case TextParserTokenType.PushColor:
                        ProcessPushColorToken(output, ref token, ref state, ref index);
                        break;

                    case TextParserTokenType.PushStyle:
                        ProcessPushStyleToken(output, ref bold, ref italic, ref token, ref state, ref index);
                        break;

                    case TextParserTokenType.PushGlyphShader:
                        ProcessPushGlyphShaderToken(output, ref token, ref state, ref index);
                        break;

                    case TextParserTokenType.PopFont:
                        ProcessPopFontToken(output, ref token, ref state, ref index);
                        break;

                    case TextParserTokenType.PopColor:
                        ProcessPopColorToken(output, ref token, ref state, ref index);
                        break;

                    case TextParserTokenType.PopStyle:
                        ProcessPopStyleToken(output, ref bold, ref italic, ref token, ref state, ref index);
                        break;

                    case TextParserTokenType.PopGlyphShader:
                        ProcessPopGlyphShaderToken(output, ref token, ref state, ref index);
                        break;
                        
                    default:
                        if (token.TokenType >= TextParserTokenType.Custom)
                        {
                            ProcessCustomCommandToken(output, ref token, ref state, ref index);
                            break;
                        }
                        else throw new InvalidOperationException(UltravioletStrings.UnrecognizedLayoutCommand.Format(token.TokenType));
                }
            }

            state.FinalizeLayout(output, ref settings);

            if (acquiredPointers)
                output.ReleasePointers();

            ClearLayoutStacks();
        }

        /// <summary>
        /// Parses a <see cref="Color"/> value from the specified string segment.
        /// </summary>
        private static Color ParseColor(StringSegment text)
        {
            if (text.Length != 8)
                throw new FormatException();

            var a = StringSegmentConversion.ParseHexadecimalInt32(text.Substring(0, 2));
            var r = StringSegmentConversion.ParseHexadecimalInt32(text.Substring(2, 2));
            var g = StringSegmentConversion.ParseHexadecimalInt32(text.Substring(4, 2));
            var b = StringSegmentConversion.ParseHexadecimalInt32(text.Substring(6, 2));

            return new Color(r, g, b, a);
        }        

        /// <summary>
        /// Clears all of the layout engine's layout parameter stacks.
        /// </summary>
        private void ClearLayoutStacks()
        {
            styleStack.Clear();
            fontStack.Clear();

            sourceString = null;
            sourceStringBuilder = null;
        }

        /// <summary>
        /// If the layout has an initial style defined, this method modifies the layout stacks to reflect it.
        /// </summary>
        private void PrepareInitialStyle(TextLayoutCommandStream output, ref Boolean bold, ref Boolean italic, ref TextLayoutSettings settings)
        {
            if (settings.InitialLayoutStyle == null)
                return;

            var initialStyle = default(TextStyle);
            var initialStyleIndex = RegisterStyleWithCommandStream(output, settings.InitialLayoutStyle, out initialStyle);
            output.WritePushStyle(new TextLayoutStyleCommand(initialStyleIndex));
            PushStyle(initialStyle, ref bold, ref italic);
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.Text"/>.
        /// </summary>
        private Boolean ProcessTextToken(TextParserTokenStream input, TextLayoutCommandStream output, SpriteFontFace currentFontFace,
            ref TextParserToken token, ref LayoutState state, ref TextLayoutSettings settings, ref Int32 index)
        {
            if (token.IsNewLine)
            {
                state.AdvanceLayoutToNextLineWithBreak(output, token.SourceLength, ref settings);
                index++;
            }
            else
            {
                if (!AccumulateText(input, output, currentFontFace, ref index, ref state, ref settings))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.Icon"/>.
        /// </summary>
        private Boolean ProcessIconToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref TextLayoutSettings settings, ref Int32 index)
        {
            var icon = default(TextIconInfo);
            var iconIndex = RegisterIconWithCommandStream(output, token.Text, out icon);
            var iconSize = MeasureToken(null, token.TokenType, token.Text);

            if (state.PositionX + iconSize.Width > (settings.Width ?? Int32.MaxValue))
                state.AdvanceLayoutToNextLine(output, ref settings);

            if (state.PositionY + iconSize.Height > (settings.Height ?? Int32.MaxValue))
                return false;

            output.WriteIcon(new TextLayoutIconCommand(iconIndex, state.PositionX, state.PositionY, (Int16)iconSize.Width, (Int16)iconSize.Height));
            state.AdvanceLineToNextCommand(iconSize.Width, iconSize.Height, 1, 1);
            index++;

            return true;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.ToggleBold"/>.
        /// </summary>
        private void ProcessToggleBoldToken(TextLayoutCommandStream output, ref Boolean bold,
            ref LayoutState state, ref Int32 index)
        {
            output.WriteToggleBold();
            state.AdvanceLineToNextCommand();
            bold = !bold;
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.ToggleItalic"/>.
        /// </summary>
        private void ProcessToggleItalicToken(TextLayoutCommandStream output, ref Boolean italic,
            ref LayoutState state, ref Int32 index)
        {
            output.WriteToggleItalic();
            state.AdvanceLineToNextCommand();
            italic = !italic;
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PushFont"/>.
        /// </summary>
        private void ProcessPushFontToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            var pushedFont = default(SpriteFont);
            var pushedFontIndex = RegisterFontWithCommandStream(output, token.Text, out pushedFont);
            output.WritePushFont(new TextLayoutFontCommand(pushedFontIndex));
            state.AdvanceLineToNextCommand();
            PushFont(pushedFont);
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PushColor"/>.
        /// </summary>
        private void ProcessPushColorToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            var pushedColor = ParseColor(token.Text);
            output.WritePushColor(new TextLayoutColorCommand(pushedColor));
            state.AdvanceLineToNextCommand();
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PushStyle"/>.
        /// </summary>
        private void ProcessPushStyleToken(TextLayoutCommandStream output, ref Boolean bold, ref Boolean italic,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            var pushedStyle = default(TextStyle);
            var pushedStyleIndex = RegisterStyleWithCommandStream(output, token.Text, out pushedStyle);
            output.WritePushStyle(new TextLayoutStyleCommand(pushedStyleIndex));
            state.AdvanceLineToNextCommand();
            PushStyle(pushedStyle, ref bold, ref italic);
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PushGlyphShader"/>.
        /// </summary>
        private void ProcessPushGlyphShaderToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            var pushedGlyphShader = default(GlyphShader);
            var pushedGlyphShaderIndex = RegisterGlyphShaderWithCommandStream(output, token.Text, out pushedGlyphShader);
            output.WritePushGlyphShader(new TextLayoutGlyphShaderCommand(pushedGlyphShaderIndex));
            state.AdvanceLineToNextCommand();
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PopFont"/>.
        /// </summary>
        private void ProcessPopFontToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            output.WritePopFont();
            state.AdvanceLineToNextCommand();
            PopFont();
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PopColor"/>.
        /// </summary>
        private void ProcessPopColorToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            output.WritePopColor();
            state.AdvanceLineToNextCommand();
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PopStyle"/>.
        /// </summary>
        private void ProcessPopStyleToken(TextLayoutCommandStream output, ref Boolean bold, ref Boolean italic,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            output.WritePopStyle();
            state.AdvanceLineToNextCommand();
            PopStyle(ref bold, ref italic);
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.PopGlyphShader"/>.
        /// </summary>
        private void ProcessPopGlyphShaderToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            output.WritePopGlyphShader();
            state.AdvanceLineToNextCommand();
            index++;
        }

        /// <summary>
        /// Processes a parser token with type <see cref="TextParserTokenType.Custom"/>.
        /// </summary>
        private void ProcessCustomCommandToken(TextLayoutCommandStream output,
            ref TextParserToken token, ref LayoutState state, ref Int32 index)
        {
            var commandID = (token.TokenType - TextParserTokenType.Custom);
            var commandValue = token.Text.IsEmpty ? default(Int32) : StringSegmentConversion.ParseInt32(token.Text);
            output.WriteCustomCommand(new TextLayoutCustomCommand(commandID, commandValue));
            state.AdvanceLineToNextCommand();
            index++;
        }

        /// <summary>
        /// Pushes a style onto the style stack.
        /// </summary>
        /// <param name="style">The style to push onto the stack.</param>
        /// <param name="bold">A value indicating whether the current font face is bold.</param>
        /// <param name="italic">A value indicating whether the current font face is italic.</param>
        private void PushStyle(TextStyle style, ref Boolean bold, ref Boolean italic)
        {
            var instance = new TextStyleInstance(style, bold, italic);
            styleStack.Push(instance);

            if (style.Font != null)
                PushFont(style.Font);

            if (style.Bold.HasValue)
                bold = style.Bold.Value;

            if (style.Italic.HasValue)
                italic = style.Italic.Value;
        }

        /// <summary>
        /// Pushes a font onto the font stack.
        /// </summary>
        /// <param name="font">The font to push onto the stack.</param>
        private void PushFont(SpriteFont font)
        {
            var scope = styleStack.Count;
            fontStack.Push(new TextStyleScoped<SpriteFont>(font, scope));
        }

        /// <summary>
        /// Pops a style off of the style stack.
        /// </summary>
        private void PopStyle(ref Boolean bold, ref Boolean italic)
        {
            if (styleStack.Count > 0)
            {
                PopStyleScope();

                var instance = styleStack.Pop();
                bold = instance.Bold;
                italic = instance.Italic;
            }
        }

        /// <summary>
        /// Pops a font off of the font stack.
        /// </summary>
        private void PopFont()
        {
            if (fontStack.Count == 0)
                return;

            var scope = styleStack.Count;
            if (fontStack.Peek().Scope != scope)
                return;

            fontStack.Pop();
        }

        /// <summary>
        /// Pops the current style scope off of the stacks.
        /// </summary>
        private void PopStyleScope()
        {
            var scope = styleStack.Count;

            while (fontStack.Count > 0 && fontStack.Peek().Scope == scope)
                fontStack.Pop();
        }

        /// <summary>
        /// Creates a <see cref="StringSegment"/> from the current source text.
        /// </summary>
        private StringSegment CreateStringSegmentFromCurrentSource(Int32 start, Int32 length)
        {
            return (sourceString != null) ?
                new StringSegment(sourceString, start, length) :
                new StringSegment(sourceStringBuilder, start, length);
        }

        /// <summary>
        /// Accumulates sequential text tokens into a single text command.
        /// </summary>
        private Boolean AccumulateText(TextParserTokenStream input, TextLayoutCommandStream output, SpriteFontFace font, ref Int32 index, ref LayoutState state, ref TextLayoutSettings settings)
        {
            var hyphenate = (settings.Options & TextLayoutOptions.Hyphenate) == TextLayoutOptions.Hyphenate;

            var availableWidth = (settings.Width ?? Int32.MaxValue);
            var x = state.PositionX;
            var y = state.PositionY;
            var width = 0;
            var height = 0;

            var accumulatedStart = input[index].Text.Start + (state.ParserTokenOffset ?? 0);
            var accumulatedLength = 0;
            var accumulatedCount = 0;

            var lineOverflow = false;
            var lineBreakPossible = false;

            var tokenText = default(StringSegment);
            var tokenNext = default(TextParserToken?);
            var tokenSize = default(Size2);

            while (index < input.Count)
            {
                var token = input[index];
                if (token.TokenType != TextParserTokenType.Text || token.IsNewLine)
                    break;

                if (!IsSegmentForCurrentSource(token.Text))
                {
                    if (accumulatedCount > 0)
                        break;

                    EmitChangeSourceIfNecessary(input, output, ref token);
                }

                tokenText = token.Text.Substring(state.ParserTokenOffset ?? 0);
                tokenNext = GetNextTextToken(input, index);
                tokenSize = MeasureToken(font, token.TokenType, tokenText, tokenNext);

                // NOTE: We assume in a couple of places that tokens sizes don't exceed Int16.MaxValue, so try to
                // avoid accumulating tokens larger than that just in case somebody is doing something dumb
                if (width + tokenSize.Width > Int16.MaxValue)
                    break;

                var overflowsLine = state.PositionX + tokenSize.Width > availableWidth;
                if (overflowsLine)
                {
                    lineOverflow = true;
                    break;
                }

                if (tokenText.Start != accumulatedStart + accumulatedLength)
                    break;

                if (token.IsWhiteSpace && (state.LineBreakCommand == null || !token.IsNonBreakingSpace))
                {
                    lineBreakPossible = true;
                    state.LineBreakCommand = output.Count;
                    state.LineBreakOffset = accumulatedLength + token.Text.Length - 1;
                }

                width = width + tokenSize.Width;
                height = Math.Max(height, tokenSize.Height);
                accumulatedLength = accumulatedLength + tokenText.Length;
                accumulatedCount++;

                state.AdvanceLineToNextCommand(tokenSize.Width, tokenSize.Height, 1, tokenText.Length);
                state.ParserTokenOffset = 0;
                state.LineLengthInCommands--;

                index++;
            }

            if (lineBreakPossible)
            {
                var preLineBreakTextStart = accumulatedStart;
                var preLineBreakTextLength = state.LineBreakOffset.Value;
                var preLineBreakText = CreateStringSegmentFromCurrentSource(preLineBreakTextStart, preLineBreakTextLength);
                var preLineBreakSize = (preLineBreakText.Length == 0) ? Size2.Zero :
                    MeasureToken(font, TextParserTokenType.Text, preLineBreakText);
                state.BrokenTextSizeBeforeBreak = preLineBreakSize;

                var postLineBreakStart = accumulatedStart + (state.LineBreakOffset.Value + 1);
                var postLineBreakLength = accumulatedLength - (state.LineBreakOffset.Value + 1);
                var postLineBreakText = CreateStringSegmentFromCurrentSource(postLineBreakStart, postLineBreakLength);
                var postLineBreakSize = (postLineBreakText.Length == 0) ? Size2.Zero :
                    MeasureToken(font, TextParserTokenType.Text, postLineBreakText, GetNextTextToken(input, index - 1));
                state.BrokenTextSizeAfterBreak = postLineBreakSize;
            }

            var bounds = new Rectangle(x, y, width, height);
            EmitTextIfNecessary(output, accumulatedStart, accumulatedLength, ref bounds, ref state);

            if (lineOverflow && !state.ReplaceLastBreakingSpaceWithLineBreak(output, ref settings))
            {
                var overflowingToken = input[index];
                if (overflowingToken.IsWhiteSpace && !overflowingToken.IsNonBreakingSpace)
                {
                    output.WriteLineBreak(new TextLayoutLineBreakCommand(1));
                    state.AdvanceLineToNextCommand(0, 0, 1, 1, isLineBreak: true);
                    state.AdvanceLayoutToNextLine(output, ref settings);

                    if (overflowingToken.Text.Length > 1)
                    {
                        state.ParserTokenOffset = 1;
                    }
                    else
                    {
                        index++;
                    }
                    return true;
                }

                if (!GetFittedSubstring(font, availableWidth, ref tokenText, ref tokenSize, ref state, hyphenate) && state.LineWidth == 0)
                    return false;

                var overflowingTokenBounds = (tokenText.Length == 0) ? Rectangle.Empty :
                    new Rectangle(state.PositionX, state.PositionY, tokenSize.Width, tokenSize.Height);

                var overflowingTextEmitted = EmitTextIfNecessary(output, tokenText.Start, tokenText.Length, ref overflowingTokenBounds, ref state);
                if (overflowingTextEmitted)
                {
                    state.AdvanceLineToNextCommand(tokenSize.Width, tokenSize.Height, 0, tokenText.Length);
                    if (hyphenate)
                    {
                        output.WriteHyphen();
                        state.AdvanceLineToNextCommand(0, 0, 1, 0);
                    }
                }

                state.ParserTokenOffset = (state.ParserTokenOffset ?? 0) + tokenText.Length;
                state.AdvanceLayoutToNextLine(output, ref settings);
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the specified string segment uses the layout engine's current source string as its source.
        /// </summary>
        private Boolean IsSegmentForCurrentSource(StringSegment segment)
        {
            if (sourceString == null && sourceStringBuilder == null)
                return false;

            if (sourceString != null)
                return segment.SourceString == sourceString;

            return segment.SourceStringBuilder == sourceStringBuilder;
        }

        /// <summary>
        /// Adds a <see cref="TextLayoutCommandType.ChangeSourceString"/> or <see cref="TextLayoutCommandType.ChangeSourceStringBuilder"/> command to the output
        /// stream if it is necessary to do so for the specified parser token.
        /// </summary>
        private Boolean EmitChangeSourceIfNecessary(TextParserTokenStream input, TextLayoutCommandStream output, ref TextParserToken token)
        {
            if (!IsSegmentForCurrentSource(token.Text))
            {
                var isFirstSource = (sourceString == null && sourceStringBuilder == null);

                sourceString = token.Text.SourceString;
                sourceStringBuilder = token.Text.SourceStringBuilder;

                // NOTE: To save memory, we can elide the first change source command if it's just going to change to the input source.
                if (!isFirstSource || input.SourceText.SourceString != sourceString || input.SourceText.SourceStringBuilder != sourceStringBuilder)
                {
                    if (sourceString != null)
                    {
                        var sourceIndex = output.RegisterSourceString(sourceString);
                        output.WriteChangeSourceString(new TextLayoutSourceStringCommand(sourceIndex));
                    }
                    else
                    {
                        var sourceIndex = output.RegisterSourceStringBuilder(sourceStringBuilder);
                        output.WriteChangeSourceStringBuilder(new TextLayoutSourceStringBuilderCommand(sourceIndex));
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a <see cref="TextLayoutCommandType.Text"/> command to the output stream if the specified span of text has a non-zero length.
        /// </summary>
        private Boolean EmitTextIfNecessary(TextLayoutCommandStream output, Int32 start, Int32 length, ref Rectangle bounds, ref LayoutState state)
        {
            if (length == 0)
                return false;

            output.WriteText(new TextLayoutTextCommand(start, length, 
                bounds.X, bounds.Y, (Int16)bounds.Width, (Int16)bounds.Height));

            state.LineLengthInCommands++;

            return true;
        }

        /// <summary>
        /// Given a string and an available space, returns the largest substring which will fit within that space.
        /// </summary>
        private Boolean GetFittedSubstring(SpriteFontFace font, Int32 maxLineWidth, ref StringSegment tokenText, ref Size2 tokenSize, ref LayoutState state, Boolean hyphenate)
        {
            var substringAvailableWidth = maxLineWidth - state.PositionX;
            var substringWidth = 0;
            var substringLength = 0;

            for (int glyphIndex = 0; glyphIndex < tokenText.Length - 1; glyphIndex++)
            {
                var glyph1 = tokenText[glyphIndex];
                var glyph2 = tokenText[glyphIndex + 1];
                var glyphWidth = 0;

                if (hyphenate)
                {
                    glyphWidth = font.MeasureGlyph(glyph1, '-').Width + font.MeasureGlyph('-').Width;
                }
                else
                {
                    glyphWidth = font.MeasureGlyph(glyph1).Width;
                }

                if (substringAvailableWidth - glyphWidth < 0)
                    break;

                var glyphSize = font.MeasureGlyph(glyph1, glyph2);
                substringAvailableWidth -= glyphSize.Width;
                substringWidth += glyphSize.Width;
                substringLength++;
            }

            tokenText = substringLength > 0 ? tokenText.Substring(0, substringLength) : StringSegment.Empty;
            tokenSize = new Size2(substringWidth, tokenSize.Height);

            return substringLength > 0;
        }

        /// <summary>
        /// Calculates the size of the specified parser token when rendered according to the current layout state.
        /// </summary>
        /// <param name="font">The current font face.</param>
        /// <param name="tokenType">The type of the current token.</param>
        /// <param name="tokenText">The text of the current token.</param>
        /// <param name="tokenNext">The next token after the current token, excluding command tokens.</param>
        /// <returns>The size of the specified token in pixels.</returns>
        private Size2 MeasureToken(SpriteFontFace font, TextParserTokenType tokenType, StringSegment tokenText, TextParserToken? tokenNext = null)
        {
            switch (tokenType)
            {
                case TextParserTokenType.Icon:
                    {
                        TextIconInfo icon;
                        if (!registeredIcons.TryGetValue(tokenText, out icon))
                            throw new InvalidOperationException(UltravioletStrings.UnrecognizedIcon.Format(tokenText));

                        return new Size2(icon.Width ?? icon.Icon.Controller.Width, icon.Height ?? icon.Icon.Controller.Height);
                    }

                case TextParserTokenType.Text:
                    {
                        var size = font.MeasureString(tokenText);
                        if (tokenNext.HasValue)
                        {
                            var tokenNextValue = tokenNext.GetValueOrDefault();
                            if (tokenNextValue.TokenType == TextParserTokenType.Text && !tokenNextValue.Text.IsEmpty && !tokenNextValue.IsNewLine)
                            {
                                var charLast = tokenText[tokenText.Length - 1];
                                var charNext = tokenNextValue.Text[0];
                                var kerning = font.Kerning.Get(charLast, charNext);
                                return new Size2(size.Width + kerning, size.Height);
                            }
                        }
                        return size;
                    }
            }
            return Size2.Zero;
        }

        /// <summary>
        /// Registers the specified style with the command stream and returns its resulting index.
        /// </summary>
        private Int16 RegisterStyleWithCommandStream(TextLayoutCommandStream output, StringSegment name, out TextStyle style)
        {
            if (!registeredStyles.TryGetValue(name, out style))
                throw new InvalidOperationException(UltravioletStrings.UnrecognizedStyle.Format(name));

            return output.RegisterStyle(name, style);
        }

        /// <summary>
        /// Registers the specified icon with the command stream and returns its resulting index.
        /// </summary>
        private Int16 RegisterIconWithCommandStream(TextLayoutCommandStream output, StringSegment name, out TextIconInfo icon)
        {
            if (!registeredIcons.TryGetValue(name, out icon))
                throw new InvalidOperationException(UltravioletStrings.UnrecognizedIcon.Format(name));

            return output.RegisterIcon(name, icon);
        }

        /// <summary>
        /// Registers the specified font with the command stream and returns its resulting index.
        /// </summary>
        private Int16 RegisterFontWithCommandStream(TextLayoutCommandStream output, StringSegment name, out SpriteFont font)
        {
            if (!registeredFonts.TryGetValue(name, out font))
                throw new InvalidOperationException(UltravioletStrings.UnrecognizedFont.Format(name));

            return output.RegisterFont(name, font);
        }

        /// <summary>
        /// Registers the specified glyph shader with the command stream and returns its resulting index.
        /// </summary>
        private Int16 RegisterGlyphShaderWithCommandStream(TextLayoutCommandStream output, StringSegment name, out GlyphShader glyphShader)
        {
            if (!registeredGlyphShaders.TryGetValue(name, out glyphShader))
                throw new InvalidOperationException(UltravioletStrings.UnrecognizedGlyphShader.Format(name));

            return output.RegisterGlyphShader(name, glyphShader);
        }

        /// <summary>
        /// Gets the next text token after the specified index, if one exists and there are no intervening
        /// visible tokens (excluding commands).
        /// </summary>
        private TextParserToken? GetNextTextToken(TextParserTokenStream input, Int32 index)
        {
            for (int i = index + 1; i < input.Count; i++)
            {
                var token = input[i];
                if (token.TokenType == TextParserTokenType.Text)
                    return token;

                if (token.TokenType == TextParserTokenType.Icon)
                    break;
            }
            return null;
        }

        /// <summary>
        /// Gets the currently active font.
        /// </summary>
        private SpriteFont GetCurrentFont(ref TextLayoutSettings settings, Boolean bold, Boolean italic, out SpriteFontFace face)
        {
            var font = (fontStack.Count == 0) ? settings.Font : fontStack.Peek().Value;
            face = font.GetFace(bold, italic);
            return font;
        }

        // Registered styles, icons, fonts, and glyph shaders.
        private readonly Dictionary<StringSegment, TextStyle> registeredStyles =
            new Dictionary<StringSegment, TextStyle>();
        private readonly Dictionary<StringSegment, TextIconInfo> registeredIcons =
            new Dictionary<StringSegment, TextIconInfo>();
        private readonly Dictionary<StringSegment, SpriteFont> registeredFonts =
            new Dictionary<StringSegment, SpriteFont>();
        private readonly Dictionary<StringSegment, GlyphShader> registeredGlyphShaders =
            new Dictionary<StringSegment, GlyphShader>();

        // Layout parameter stacks.
        private readonly Stack<TextStyleInstance> styleStack = new Stack<TextStyleInstance>();
        private readonly Stack<TextStyleScoped<SpriteFont>> fontStack = new Stack<TextStyleScoped<SpriteFont>>();

        // The current source string.
        private String sourceString;
        private StringBuilder sourceStringBuilder;
    }
}
