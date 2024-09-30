import os
import json
from opencc import OpenCC

# 初始化简体转繁体的转换器
cc = OpenCC('s2t')  # 简体到繁体的转换

# 目标目录
target_directory = r'E:\FDisk\GitHubProject\unblockKlotski\unblockKlotski\Assets\NavySoftUnblockWood\Resources\GameMode'

# 遍历文件夹，找到所有json文件
for root, dirs, files in os.walk(target_directory):
    for file in files:
        if file.endswith(".json"):
            file_path = os.path.join(root, file)
            try:
                # 打开并读取 JSON 文件
                with open(file_path, 'r', encoding='utf-8') as f:
                    data = json.load(f)

                # 将所有字符串内容转换为繁体中文
                def convert_to_traditional(value):
                    if isinstance(value, str):
                        return cc.convert(value)
                    elif isinstance(value, list):
                        return [convert_to_traditional(v) for v in value]
                    elif isinstance(value, dict):
                        return {convert_to_traditional(k): convert_to_traditional(v) for k, v in value.items()}
                    else:
                        return value

                # 转换 JSON 数据中的简体中文
                converted_data = convert_to_traditional(data)

                # 将转换后的内容写回文件
                with open(file_path, 'w', encoding='utf-8') as f:
                    json.dump(converted_data, f, ensure_ascii=False, indent=4)

                print(f"已转换文件: {file_path}")
            except Exception as e:
                print(f"处理文件时出错 {file_path}: {e}")
