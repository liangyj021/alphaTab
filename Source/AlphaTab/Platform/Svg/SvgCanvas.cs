﻿/*
 * This file is part of alphaTab.
 * Copyright (c) 2014, Daniel Kuschny and Contributors, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or at your option any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using AlphaTab.Collections;
using AlphaTab.Platform.Model;
using AlphaTab.Rendering;
using AlphaTab.Rendering.Glyphs;

namespace AlphaTab.Platform.Svg
{
    /// <summary>
    ///  A canvas implementation storing SVG data
    /// </summary>
    public abstract class SvgCanvas : ICanvas, IPathCanvas
    {
        protected const float BlurCorrection = 0.5f;

        protected StringBuilder Buffer;
        private StringBuilder _currentPath;
        private bool _currentPathIsEmpty;

        public Color Color { get; set; }
        public float LineWidth { get; set; }
        public Font Font { get; set; }
        public TextAlign TextAlign { get; set; }
        public TextBaseline TextBaseline { get; set; }
        public RenderingResources Resources { get; set; }

        public SvgCanvas()
        {
            _currentPath = new StringBuilder();
            _currentPathIsEmpty = true;
            Color = new Color(255, 255, 255);
            LineWidth = 1;
            Font = new Font("Arial", 10);
            TextAlign = TextAlign.Left;
            TextBaseline = TextBaseline.Default;
        }

        public virtual void BeginRender(float width, float height)
        {
            Buffer = new StringBuilder();

            Buffer.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"");
            Buffer.Append(width);
            Buffer.Append("px\" height=\"");
            Buffer.Append(height);
            Buffer.Append("px\" class=\"alphaTabSurfaceSvg\">\n");
            _currentPath = new StringBuilder();
            _currentPathIsEmpty = true;
        }

        public virtual object EndRender()
        {
            Buffer.Append("</svg>");
            return Buffer.ToString();
        }

        public void FillRect(float x, float y, float w, float h)
        {
            Buffer.Append("<rect x=\"");
            Buffer.Append(x - BlurCorrection);
            Buffer.Append("\" y=\"");
            Buffer.Append(y - BlurCorrection);
            Buffer.Append("\" width=\"");
            Buffer.Append(w);
            Buffer.Append("\" height=\"");
            Buffer.Append(h);
            Buffer.Append("\" style=\"fill:");
            Buffer.Append(Color.RGBA);
            Buffer.Append(";\" />\n");
        }

        public void StrokeRect(float x, float y, float w, float h)
        {
            Buffer.Append("<rect x=\"");
            Buffer.Append(x - BlurCorrection);
            Buffer.Append("\" y=\"");
            Buffer.Append(y - BlurCorrection);
            Buffer.Append("\" width=\"");
            Buffer.Append(w);
            Buffer.Append("\" height=\"");
            Buffer.Append(h);
            Buffer.Append("\" style=\"stroke:");
            Buffer.Append(Color.RGBA);
            Buffer.Append("; stroke-width:");
            Buffer.Append(LineWidth);
            Buffer.Append("; fill:transparent");
            Buffer.Append(";\" />\n");
        }

        public void BeginPath()
        {
        }

        public void ClosePath()
        {
            _currentPath.Append(" z");
        }

        public void MoveTo(float x, float y)
        {
            _currentPath.Append(" M");
            _currentPath.Append(x - BlurCorrection);
            _currentPath.Append(",");
            _currentPath.Append(y - BlurCorrection);
        }

        public void LineTo(float x, float y)
        {
            _currentPathIsEmpty = false;
            _currentPath.Append(" L");
            _currentPath.Append(x - BlurCorrection);
            _currentPath.Append(",");
            _currentPath.Append(y - BlurCorrection);
        }

        public void QuadraticCurveTo(float cpx, float cpy, float x, float y)
        {
            _currentPathIsEmpty = false;
            _currentPath.Append(" Q");
            _currentPath.Append(cpx);
            _currentPath.Append(",");
            _currentPath.Append(cpy);
            _currentPath.Append(",");
            _currentPath.Append(x);
            _currentPath.Append(",");
            _currentPath.Append(y);
        }

        public void BezierCurveTo(float cp1x, float cp1y, float cp2x, float cp2y, float x, float y)
        {
            _currentPathIsEmpty = false;
            _currentPath.Append(" C");
            _currentPath.Append(cp1x);
            _currentPath.Append(",");
            _currentPath.Append(cp1y);
            _currentPath.Append(",");
            _currentPath.Append(cp2x);
            _currentPath.Append(",");
            _currentPath.Append(cp2y);
            _currentPath.Append(",");
            _currentPath.Append(x);
            _currentPath.Append(",");
            _currentPath.Append(y);
        }

        public void FillCircle(float x, float y, float radius)
        {
            _currentPathIsEmpty = false;
            // 
            // M0,250 A1,1 0 0,0 500,250 A1,1 0 0,0 0,250 z
            _currentPath.Append(" M");
            _currentPath.Append(x - radius);
            _currentPath.Append(",");
            _currentPath.Append(y);

            _currentPath.Append(" A1,1 0 0,0 ");
            _currentPath.Append(x + radius);
            _currentPath.Append(",");
            _currentPath.Append(y);

            _currentPath.Append(" A1,1 0 0,0 ");
            _currentPath.Append(x - radius);
            _currentPath.Append(",");
            _currentPath.Append(y);

            _currentPath.Append(" z");

            Fill();
        }

        public void Fill()
        {
            if (!_currentPathIsEmpty)
            {
                Buffer.Append("<path d=\"");
                Buffer.Append(_currentPath.ToString());
                Buffer.Append("\" style=\"fill:");
                Buffer.Append(Color.RGBA);
                Buffer.Append("\" stroke=\"none\"/>\n");
            }
            _currentPath = new StringBuilder();
            _currentPathIsEmpty = true;
        }

        public void Stroke()
        {
            if (!_currentPathIsEmpty)
            {
                Buffer.Append("<path d=\"");
                Buffer.Append(_currentPath.ToString());
                Buffer.Append("\" style=\"stroke:");
                Buffer.Append(Color.RGBA);
                Buffer.Append("; stroke-width:");
                Buffer.Append(LineWidth);
                Buffer.Append(";\" fill=\"none\" />\n");
            }
            _currentPath = new StringBuilder();
            _currentPathIsEmpty = true;
        }

        public void FillText(string text, float x, float y)
        {
            Buffer.Append("<text x=\"");
            Buffer.Append(x);
            Buffer.Append("\" y=\"");
            Buffer.Append(y + GetSvgBaseLineOffset());
            Buffer.Append("\" style=\"font:");
            Buffer.Append(Font.ToCssString());
            Buffer.Append("; fill:");
            Buffer.Append(Color.RGBA);
            Buffer.Append(";\" ");
            Buffer.Append(" dominant-baseline=\"");
            Buffer.Append(GetSvgBaseLine());
            Buffer.Append("\" text-anchor=\"");
            Buffer.Append(GetSvgTextAlignment());
            Buffer.Append("\">\n");
            Buffer.Append(text);
            Buffer.Append("</text>\n");
        }

        private string GetSvgTextAlignment()
        {
            switch (TextAlign)
            {
                case TextAlign.Left: return "start";
                case TextAlign.Center: return "middle";
                case TextAlign.Right: return "end";
            }
            return "";
        }

        protected float GetSvgBaseLineOffset()
        {
            switch (TextBaseline)
            {
                case TextBaseline.Top: return 0;
                case TextBaseline.Middle: return 0;
                case TextBaseline.Bottom: return 0;
                default: return Font.Size;
            }
        }

        private string GetSvgBaseLine()
        {
            switch (TextBaseline)
            {
                case TextBaseline.Top: return "top";
                case TextBaseline.Middle: return "middle";
                case TextBaseline.Bottom: return "bottom";
                default: return "top";
            }
        }

        public float MeasureText(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            var font = SupportedFonts.Arial;
            if (Font.Family.Contains("Times"))
            {
                font = SupportedFonts.TimesNewRoman;
            }
            return FontSizes.MeasureString(text, font, Font.Size, Font.Style);
        }

        public abstract void FillMusicFontSymbol(float x, float y, float scale, MusicFontSymbol symbol);


        public virtual object OnPreRender()
        {
            // nothing to do
            return null;
        }

        public virtual object OnRenderFinished()
        {
            // nothing to do
            return null;
        }
    }
}