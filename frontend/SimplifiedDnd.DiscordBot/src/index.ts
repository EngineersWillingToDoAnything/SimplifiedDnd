import dotenv from 'dotenv';

import Bot from './abstractions/bot';
import DndApiService from './infrastructure/api-service';

dotenv.config();

const dndService = new DndApiService();
const client = new Bot(dndService);

client.load();
client.login(process.env.DISCORD_BOT_TOKEN);