'use strict';

$(document).ready(function () { 

    let hull = new DynamicConvexHull();
    let all_points = [];

    $("#canvas").on("click", function(e) {
        let to_add = new Point(e.pageX, -e.pageY);
        let found = false;

        all_points.forEach(point => {
            if (Math.sqrt((point.x - to_add.x) * (point.x - to_add.x) + (point.y - to_add.y) * (point.y - to_add.y)) <= 14) {                
                found = true;
            }
            if (Math.sqrt((point.x - to_add.x) * (point.x - to_add.x) + (point.y - to_add.y) * (point.y - to_add.y)) <= 10) {                

                hull.delete_point(point);

                point.x = -100;
                point.y = -100;
            }
        });

        if (!found) {
            hull.insert_point(to_add);
            all_points.push(to_add);
        }

        let canvas = document.getElementById("canvas");
        let ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, 1000, 1000);

        let list = hull.get_hull_as_array();


        ctx.fillStyle = '#FFC373';
        all_points.forEach(point => {
            ctx.beginPath();
            ctx.arc(point.x, -point.y, 7, 0, 2*Math.PI);
            ctx.fill();
        });

        ctx.strokeStyle = '#4A2A82';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(list[list.length - 1].x, -list[list.length - 1].y);
        list.forEach(point => {
            ctx.lineTo(point.x, -point.y);
            ctx.moveTo(point.x, -point.y);
        });
        ctx.stroke();

        ctx.fillStyle = '#4A2A82';
        list.forEach(point => {
            ctx.beginPath();
            ctx.arc(point.x, -point.y, 7, 0, 2*Math.PI);
            ctx.fill();
        });
    });


 });


