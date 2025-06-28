import {
  ActionRowBuilder,
  CacheType,
  ChatInputCommandInteraction,
  SlashCommandBuilder,
  SlashCommandOptionsOnlyBuilder,
  StringSelectMenuBuilder,
  StringSelectMenuInteraction,
  StringSelectMenuOptionBuilder,
} from 'discord.js';

import DndCommandHandler from '../abstractions/dnd/dnd-command-handler';
import { MaybeType } from '../abstractions/maybe-types';

enum CommandOptions {
  CharacterName = 'name',
}

export default class NewCharacterCommandHandler extends DndCommandHandler {
  readonly command: SlashCommandOptionsOnlyBuilder;

  constructor() {
    super();
    this.command = new SlashCommandBuilder()
      .setName('new-character')
      .setDescription('Create a new character assigned to the user')
      .addStringOption(option =>
        option
          .setName(CommandOptions.CharacterName)
          .setDescription('The name of the character')
          .setRequired(true),
      );
  }

  getCommand(): SlashCommandBuilder {
    return this.command as SlashCommandBuilder;
  }

  protected async handleCommand(
    interaction: ChatInputCommandInteraction<CacheType>,
  ): Promise<void> {
    const characterName = interaction.options.getString(
      CommandOptions.CharacterName,
      true,
    );

    const specieName = await this.getSpecie(interaction);
    const className = await this.getClass(interaction);
    if (!specieName || !className) {
      await interaction.editReply({
        components: [],
        content: 'Confirmation not received within 1 minute, cancelling',
      });
      return;
    }

    const characterId = await this.service?.createCharacter({
      className,
      name: characterName,
      playerId: interaction.user.id,
      specieName,
    });

    if (characterId?.type === MaybeType.DomainError) {
      await interaction.editReply({
        components: [],
        content: characterId.detail,
      });
      return;
    }

    await interaction.editReply({
      components: [],
      content: `Character \`${characterName}\` has been created successfully!`,
    });
  }

  private getSpecieSelectMenu() {
    return new StringSelectMenuBuilder()
      .setCustomId('specie-menu')
      .setPlaceholder('Make a choice!')
      .addOptions(
        new StringSelectMenuOptionBuilder()
          .setLabel('Dragonborn')
          .setDescription('Shaped by draconic gods or the dragons themselves')
          .setValue('Dragonborn'),
        new StringSelectMenuOptionBuilder()
          .setLabel('Barbarian')
          .setValue('Barbarian'),
        new StringSelectMenuOptionBuilder()
          .setLabel('Human')
          .setDescription('The common specie of the world.')
          .setValue('Human'),
      );
  }

  private async getSpecie(
    interaction: ChatInputCommandInteraction<CacheType>,
  ): Promise<string | undefined> {
    const row = new ActionRowBuilder<StringSelectMenuBuilder>().addComponents(
      this.getSpecieSelectMenu(),
    );

    const response = await interaction.reply({
      components: [row],
      content: "Select your character's `specie`:",
      withResponse: true,
    });

    try {
      const collected = await response.resource?.message?.awaitMessageComponent(
        {
          filter: i => i.user.id === interaction.user.id,
          time: 60_000,
        },
      );

      await collected?.deferUpdate();
      return (collected as StringSelectMenuInteraction).values[0];
    } catch {
      return undefined;
    }
  }

  private getClassSelectMenu() {
    return new StringSelectMenuBuilder()
      .setCustomId('class-menu')
      .setPlaceholder('Make a choice!')
      .addOptions(
        new StringSelectMenuOptionBuilder()
          .setLabel('Artificer')
          .setValue('Artificer'),
        new StringSelectMenuOptionBuilder()
          .setLabel('Barbarian')
          .setValue('Barbarian'),
        new StringSelectMenuOptionBuilder().setLabel('Bard').setValue('Bard'),
      );
  }

  private async getClass(
    interaction: ChatInputCommandInteraction<CacheType>,
  ): Promise<string | undefined> {
    const row = new ActionRowBuilder<StringSelectMenuBuilder>().addComponents(
      this.getClassSelectMenu(),
    );

    const response = await interaction.editReply({
      components: [row],
      content: "Perfect, now select your character's `class`:",
    });

    try {
      const collected = await response.awaitMessageComponent({
        filter: i => i.user.id === interaction.user.id,
        time: 60_000,
      });

      await collected.deferUpdate();
      return (collected as StringSelectMenuInteraction).values[0];
    } catch {
      return undefined;
    }
  }
}
