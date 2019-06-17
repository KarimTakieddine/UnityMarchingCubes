/*-------------------------------------------------------------------------------------*\

##	Copyright © 2019 Karim Takieddine
##
##	Permission is hereby granted, free of charge, to any person obtaining a copy
##	of this software and associated documentation files (the "Software"), to deal
##	in the Software without restriction, including without limitation the rights
##	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
##	of the Software, and to permit persons to whom the Software is furnished to do so,
##	subject to the following conditions:
##
##	The above copyright notice and this permission notice shall be included in all
##	copies or substantial portions of the Software.
##
##	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
##	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
##	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
##	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
##	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
##	OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*-------------------------------------------------------------------------------------*/

﻿using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour
{
    public List<Cube> Cells { get; private set; }

    public Color CellColor;

    public int CellCountX, CellCountY, CellCountZ;
    public float CellSize;

    public bool Draw;
    
    private void Awake()
    {
        if (CellCountX < 0)
        {
            CellCountX = 0;
        }

        if (CellCountY < 0)
        {
            CellCountY = 0;
        }

        if (CellCountZ < 0)
        {
            CellCountZ = 0;
        }

        Cells = new List<Cube>( CellCountX * CellCountY * CellCountZ );

        LoadCells();
    }

    private void LoadCells()
    {
        if (Cells.Count > 0)
        {
            Cells.Clear();
        }

        float halfSize = CellSize * 0.5f;

        for (int i = 0; i < CellCountZ; ++i)
        {
            for (int j = 0; j < CellCountY; ++j)
            {
                for (int k = 0; k < CellCountX; ++k)
                {
                    Cells.Add(
                        new Cube(
                            CellSize,
                            new Vector3(
                                k * CellSize,
                                j * CellSize,
                                i * CellSize
                            )
                        )
                    );
                }
            }
        }
    }

    public int GetGridIndex(
        float position,
        int cell_count
    )
    {
        return Mathf.Clamp(
            (int)( position / CellSize ),
            0,
            cell_count - 1
        );
    }

    public int GetGridIndex(
        int x,
        int y,
        int z
    )
    {
        return ( CellCountX * CellCountY * z ) + ( CellCountX * y ) + x;
    }

    public int GetGridIndex(Vector3 position)
    {
        return GetGridIndex(
            GetGridIndex(position.x, CellCountX),
            GetGridIndex(position.y, CellCountX),
            GetGridIndex(position.z, CellCountZ)
        );
    }

    public static void DrawEdge(
        in Edge edge,
        Color color
    )
    {
        Debug.DrawLine(edge.First, edge.Second, color);
    }

    public static void DrawCell(
        in Cube cell,
        Color color
    )
    {
        DrawEdge(cell.BottomForward, color);
        DrawEdge(cell.BottomRight, color);
        DrawEdge(cell.BottomBack, color);
        DrawEdge(cell.BottomLeft, color);

        DrawEdge(cell.TopForward, color);
        DrawEdge(cell.TopRight, color);
        DrawEdge(cell.TopBack, color);
        DrawEdge(cell.TopLeft, color);
        
        DrawEdge(cell.UpLeftForward, color);
        DrawEdge(cell.UpRightForward, color);
        DrawEdge(cell.UpRightBack, color);
        DrawEdge(cell.UpLeftBack, color);
    }

    private void Update()
    {
        if (Draw)
        {
            for (int i = 0; i < Cells.Count; ++i)
            {
                DrawCell(Cells[i], CellColor);
            }
        }
    }
};