namespace SI_CSP.Problems
{
    /*zaimplementowane heury:
     * Zmienna:
     *  Sudoku - def, 
     *          rand, 
     *          dynamiczna heura zmiennej o najmniejszej liczbie legalnych wartości
     *  Jolka - def, 
     *          rand, 
     *          najbardziej ograniczonej zmiennej - zmienna, która przecina się z najwieksza liczba innych słów,
     *          po kolei wiersze, 
     *          od lewego górnego rogu, 
     *          najmniejszej dziedziny,
     *          od lewego górnego i wg dziedziny
     *          dynamiczna heura zmiennej o najmniejszej liczbie legalnych wartości
     *          
     *Wartość:
     *  Sudoku - def, 
     *          rand, 
     *          statyczna najmniej ograniczającej wartości - domena posortowana rosnąco wg liczby wartości w dziedzinach wszystkich 
     *                                                        powiązanych zmiennych, jakie blokuje dana wartość
     *  Jolka - def, 
     *          rand, 
     *          statyczna najmniej ograniczającej wartości - domena posortowana rosnąco wg ilości słów w dziedzinach wszystkich 
     *                                                        powiązanych zmiennych, jakie blokuje dane słowo
    */
    public enum VariableHeuristics
    {
        DEFINITION_ORDER,
        RANDOM,
        MOST_CONSTRAINED,
        ROWS_DESCENDING,
        FROM_TOP_LEFT_CORNER,
        LEAST_DOMAIN,
        FROM_TOP_LEFT_LEAST_DOMAIN,
        MIN_REMAINING_VALUES_DYN
    }

    public enum ValueHeuristics
    {
        DEFINITION_ORDER,
        RANDOM,
        LEAST_CONSTRAINING_VALUE
    }
}
