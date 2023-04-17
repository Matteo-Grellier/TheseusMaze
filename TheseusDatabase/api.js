const Koa = require('koa');
const Router = require('koa-router');
const Logger = require('koa-logger');
const { Pool } = require('pg');
const fs = require('fs');

const app = new Koa();
const router = new Router();

app.pool = new Pool({
    user: 'matteo',
    host: 'localhost',
    database: 'theseusdatabase',
    password: 'matteo', // Password is empty be default
    port: 5432, // Default port
});

router.get('/apiGetAll', async (ctx) => {
    const client = await app.pool.connect();
    try {
        const res = await client.query('SELECT * FROM maze');
        const mazes = res.rows;
        const mazesWithRooms = await Promise.all(mazes.map(async (maze) => {
            const res = await client.query('SELECT * FROM room WHERE mazeid = $1', [maze.mazeid]);
            const rooms = res.rows;
            const roomsWithCases = await Promise.all(rooms.map(async (room) => {
                const res = await client.query('SELECT * FROM cell WHERE roomid = $1', [room.roomid]);
                const cases = res.rows;
                return { ...room, cases };
            }));
            return { ...maze, rooms: roomsWithCases };
        }));
        // create a JSON file containing the mazes at ../Scripts
        fs.writeFile('../Assets/Scripts/maze.json', JSON.stringify(mazesWithRooms), (err) => {
            if (err) throw err;
            console.log('The file has been saved!');
        });
        ctx.body = mazesWithRooms;
    }
    finally {
        client.release();
    }
});

// Development logging
app.use(Logger());
app.use(router.routes());
app.use(router.allowedMethods());

app.listen(3001, () => {
    console.log('Server running on port 3001');
});
