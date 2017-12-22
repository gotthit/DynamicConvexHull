'use strict';

function get_treap_size(some_treap) {
    return some_treap == null ? 0 : some_treap.size;
}

function split_treap_by_index(current_treap, index) {
    if (current_treap == null) {
        return {left_treap: null, right_treap: null};
    }

    if (get_treap_size(current_treap.left_treap) >= index) {
        let result = split_treap_by_index(current_treap.left_treap, index);
        current_treap.left_treap = result.right_treap;
        current_treap.update();
        return {
            left_treap: result.left_treap, 
            right_treap: current_treap
        };
    } else {
        let result = split_treap_by_index(current_treap.right_treap, index - get_treap_size(current_treap.left_treap) - 1);
        current_treap.right_treap = result.left_treap;
        current_treap.update();
        return {
            left_treap: current_treap, 
            right_treap: result.right_treap
        };
    }
}

function merge_treaps(left_treap, right_treap) {
    if (left_treap == null) {
        return right_treap;
    }
    if (right_treap == null) {
        return left_treap;
    }
    if (get_random_in_scope(0, left_treap.size + right_treap.size) < left_treap.size) {
        left_treap.right_treap = merge_treaps(left_treap.right_treap, right_treap);
        left_treap.update();
        return left_treap;
    } else {
        right_treap.left_treap = merge_treaps(left_treap, right_treap.left_treap);
        right_treap.update();
        return right_treap;
    }
}

class Treap {

    constructor(key, left_treap = null, right_treap = null) {
        this.key = key;
        this.random_key = Math.random();
        this.left_treap = left_treap;
        this.right_treap = right_treap;

        this.size = 1;
        this.max_point = key;
        this.min_point = key;

        this.update();
    }

    update() {
        this.size = get_treap_size(this.left_treap) + get_treap_size(this.right_treap) + 1;

        this.max_point = this.key;
        this.min_point = this.key;

        if (this.left_treap != null) {
            this.max_point = Math.max(this.max_point, this.left_treap.max_point);
            this.min_point = Math.min(this.min_point, this.left_treap.min_point);
        }
        if (this.right_treap != null) {
            this.max_point = Math.max(this.max_point, this.right_treap.max_point);
            this.min_point = Math.min(this.min_point, this.right_treap.min_point);
        }
    }

    get_array() {
        let left_array = this.left_treap == null ? [] : this.left_treap.get_array();
        let right_array = this.right_treap == null ? [] : this.right_treap.get_array();

        return left_array.concat([this.key], right_array);
    }
}