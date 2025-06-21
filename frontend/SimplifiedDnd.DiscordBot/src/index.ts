import dotenv from 'dotenv';

import Bot from './abstractions/bot';

dotenv.config();

const client = new Bot();

client.load();
client.login(process.env.DISCORD_BOT_TOKEN);