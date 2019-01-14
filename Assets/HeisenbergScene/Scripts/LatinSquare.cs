using System.Collections;
using System.Collections.Generic;

public class LatinSquare
{

    private int[][] Square { get; }

    public LatinSquare(int n)
    {

        // Algorithm inspired by http://euanfreeman.co.uk/balanced-latin-squares/

        int[][] tmp = new int[n][];
        for(int i = 0; i < n; i++)
        {
            tmp[i] = new int[n];
        }

        tmp[0][0] = 1;
        tmp[0][1] = 2;

        for(int i = 2, j = 3, k = 0; i < n; i++)
        {
            if(i%2==1)
            {
                tmp[0][i] = j++;
            }
            else
            {
                tmp[0][i] = n - (k++);
            }
        }

        for(int i = 1; i <= n; i++)
        {
            tmp[i - 1][0] = i;
        }

        for(int r = 1; r < n; r++)
        {
            for(int c = 1; c < n; c++)
            {
                tmp[r][c] = (tmp[r - 1][c] + 1) % n;
                if(tmp[r][c] == 0)
                {
                    tmp[r][c] = n;
                }
            }
        }

        this.Square = tmp;
    }

    public int[] GetColumn(int id)
    {
        return (0 <= id) && (id <= 8) ? this.Square[id] : new int[] { };
    }

    public bool[] GetStates(int state)
    {
        bool[] o = new bool[3];
        // State 0: TRUE => Sitzend | FALSE => Stehend
        o[0] = state <= 4;

        // State 1: TRUE => Ausgestreckt | FALSE => Angelegt
        o[1] = new List<int>() { 1, 2, 5, 6 }.IndexOf(state) != -1;

        // State 2: TRUE => 6DOF | FALSE => 3DOF
        o[2] = (state % 2) == 1;

        return o;
    }

    public string PrintState(bool[] states)
    {
        string o = "";
        o += states[0] ? "Sitzend\r\n" : "Stehend\r\n";
        o += states[1] ? "ausgestreckt\r\n" : "angelegt\r\n";
        o += states[2] ? "6DOF\r\n" : "3DOF\r\n";
        return o;
    }
    
}
