'use strict';

var PointPosition = {
    Left: "Left",
    Right: "Right",
    On: "On"
}

function get_max_point(first_point, second_point) {
    if (first_point.compare_to(second_point) > 0) {
        return first_point;
    }
    return second_point;
}

function get_min_point(first_point, second_point) {
    if (first_point.compare_to(second_point) < 0) {
        return first_point;
    }
    return second_point;
}

class Point {

    constructor(x, y) {
        this.x = x;
        this.y = y;
    }

    compare_to(other_point) {
        if (this.y < other_point.y || (this.y == other_point.y && this.x < other_point.x)) {
            return -1;
        } 
        if (this.y > other_point.y || (this.y == other_point.y && this.x > other_point.x)) {
            return 1;
        }
        return 0;
    }
}