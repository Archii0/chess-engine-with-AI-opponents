Public Class Knight
    Inherits Piece
    Implements IChessPiece

    Public Movements = New Integer() {6, 10, 15, 17, -6, -10, -15, -17}
    Public Sub SetIcon(x As Integer, y As Integer) Implements IChessPiece.SetIcon
        If colour = Piece.White Then
            pieceImage = My.Resources.WhiteKnight
        ElseIf colour = Piece.Black Then
            pieceImage = My.Resources.BlackKnight
        End If

        With pieceRectangle
            .X = x
            .Y = y
            .Width = pieceImage.Width
            .Height = pieceImage.Height
        End With
    End Sub
    Public Function GetPieceType() As Integer Implements IChessPiece.GetPieceType
        Return Piece.Knight
    End Function
    Public Function GetMovementArray() As Integer() Implements IChessPiece.GetMovementArray
        Return Movements
    End Function
End Class
