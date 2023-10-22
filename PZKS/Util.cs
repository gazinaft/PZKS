namespace PZKS;

public class Util
{
    public static int GetClosingPar(List<Token> tokens, int start = 0)
    {
        var openCount = 0;
        for (int i = start + 1; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType == TokenType.LeftParent)
            {
                openCount += 1;
            }

            if (tokens[i].TokenType != TokenType.RightParent) continue;
            
            if (openCount == 0)
            {
                return i;
            }

            openCount -= 1;
        }

        return 0;
    }
}