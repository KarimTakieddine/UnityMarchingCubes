using System.Collections.Generic;
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