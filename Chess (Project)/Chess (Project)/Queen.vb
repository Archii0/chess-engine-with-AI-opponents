Public Class Queen
    Inherits Piece
    Implements IChessPiece

    Public Movements = New Integer() {1, 7, 8, 9, -1, -7, -8, -9}
    Public Sub SetIcon(x As Integer, y As Integer) Implements IChessPiece.SetIcon
        If colour = Piece.White Then
            pieceImage = My.Resources.WhiteQueen
        ElseIf colour = Piece.Black Then
            pieceImage = My.Resources.BlackQueen
        End If

        With pieceRectangle
            .X = x
            .Y = y
            .Width = pieceImage.Width
            .Height = pieceImage.Height
        End With
    End Sub
    Public Function GetPieceType() As Integer Implements IChessPiece.GetPieceType
        Return Piece.Queen
    End Function
    Public Function GetMovementArray() As Integer() Implements IChessPiece.GetMovementArray
        Return Movements
    End Function
End Class
