shader_type spatial;
render_mode skip_vertex_transform;
// unshaded - чтобы не было реалистичных теней от Godot, мы сами зададим цвет
// skip_vertex_transform - мы сами будем двигать вершины
// cull_disabled - чтобы видеть стены с двух сторон (для PS1 это норма)

uniform sampler2D albedo_texture : source_color, filter_nearest; // Текстура объекта
uniform vec4 tint_color : source_color = vec4(1.0, 1.0, 1.0, 1.0); // Цвет для подкраски
uniform float jitter_intensity : hint_range(0.0, 1.0) = 0.5; // Сила дрожания
uniform vec2 resolution = vec2(320.0, 240.0); // Разрешение нашего "экрана" PS1

void vertex() {
    // 1. Переводим вершину в координаты экрана (Clip Space)
    vec4 position = PROJECTION_MATRIX * MODELVIEW_MATRIX * vec4(VERTEX, 1.0);

    // 2. Магия PS1: Округляем позицию до сетки экрана
    // Чем меньше разрешение, тем сильнее "ступеньки"
    vec2 grid_size = resolution / (1.0 + jitter_intensity);
    position.xy = round(position.xy / position.w * grid_size) / grid_size * position.w;

    // 3. Применяем итоговую позицию
    POSITION = position;
    
    // Иногда для аутентичности нужно исказить UV, но пока оставим так
    UV = UV;
}

void fragment() {
    // Берем цвет из текстуры
    vec4 tex_color = texture(albedo_texture, UV);
    
    // Накладываем цвет материала (если есть)
    ALBEDO = tex_color.rgb * tint_color.rgb;
    
    // ВАЖНО: Имитация 15-битного цвета (Color Banding)
    // PS1 не умела показывать плавные градиенты
    float color_depth = 32.0; // 32 оттенка на канал
    ALBEDO = floor(ALBEDO * color_depth) / color_depth;
}