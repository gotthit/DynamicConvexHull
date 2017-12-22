'use strict';

function get_random_in_scope(min, max) {
    return Math.random() * (max - min) + min;
}

function determine_posotion(begin_vector, end_vector, to_determine) {

    let vector_multiplication_result = (to_determine.x - begin_vector.x) * (end_vector.y - begin_vector.y) 
                                        - (to_determine.y - begin_vector.y) * (end_vector.x - begin_vector.x);
    
    if (vector_multiplication_result > 0) {
        return PointPosition.Right;
    } else if (vector_multiplication_result < 0) {
        return PointPosition.Left;
    } else {
        return PointPosition.On;
    }
}